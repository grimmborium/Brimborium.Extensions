namespace Brimborium.Extensions.Http {
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Net.Http;
    using System.Threading;

    public class HttpClientRecycler {
        private HttpClientGenerator _HttpClientGenerator;
        private ReuseRecycleHandler _ReuseRecycleHandler;
        private readonly HttpClientConfiguration _Configuration;
        private int _Usage;
        private Timer _Timer;

        public HttpClientRecycler(
            HttpClientGenerator httpClientGenerator,
            HttpClientConfiguration configuration
            ) {
            this._HttpClientGenerator = httpClientGenerator;
            this._Configuration = configuration;
        }

        public HttpClient CreateHttpClient() {
            var handler = this._ReuseRecycleHandler;
            if (handler == null) {
                lock (this) {
                    handler = Volatile.Read(ref this._ReuseRecycleHandler);
                    if (handler == null) {
                        handler = this.CreateReuseRecycleHandler();
                        this._ReuseRecycleHandler = handler;
                    }
                }
            }
            var trigger = new DisposeRecycleHandler(handler, this);
            var httpClient = new HttpClient(trigger, true);
            return httpClient;
        }

        private ReuseRecycleHandler CreateReuseRecycleHandler() {
            // within lock
            System.Threading.Interlocked.Increment(ref this._Usage);
            //
            try {
                var services = this._HttpClientGenerator.Services;
                var scope = (IServiceScope)null;

                if (!this._Configuration.SuppressHandlerScope) {
                    scope = this._HttpClientGenerator.ScopeFactory.CreateScope();
                    services = scope.ServiceProvider;
                }

                var builder = services.GetRequiredService<IHttpMessageHandlerBuilder>();
                builder.SetConfiguration(this._Configuration);
                builder.ApplyConfig();
                var messageHandler = builder.Build();

                var loggerName = this._Configuration.Name;

                var outerLogger = this._HttpClientGenerator.LoggerFactory.CreateLogger($"System.Net.Http.HttpClient.{loggerName}.Outer");

                var handler = new ReuseRecycleHandler(
                    messageHandler,
                    this._Configuration,
                    scope,
                    outerLogger
                    );

                System.Threading.Interlocked.Exchange(ref this._Timer, null)?.Dispose();

                return handler;
            } catch {
                System.Threading.Interlocked.Decrement(ref this._Usage);
                throw;
            }
        }

        internal void OnDispose(DisposeRecycleHandler disposeRecycleHandler) {
            if (System.Threading.Interlocked.Decrement(ref this._Usage) == 0) {
                // Create Timer
                lock (this) {
                    if (Volatile.Read(ref this._Timer) == null) {
                        var timer = NonCapturingTimer.Create(this.OnTimer, null, TimeSpan.FromMinutes(2), TimeSpan.FromMinutes(1));
                        if (System.Threading.Interlocked.CompareExchange(ref this._Timer, timer, null) is null) {
                            // OK
                        } else {
                            timer.Dispose();
                        }
                    }
                }
            }
        }

        private void OnTimer(object state) {
            if (this._Usage == 0) {
                lock (this) {
                    var handler = System.Threading.Interlocked.Exchange(ref this._ReuseRecycleHandler, null);
                    System.Threading.Interlocked.Exchange(ref this._Timer, null)?.Dispose();
                    handler?.Dispose();
                }
            } else {
                System.Threading.Interlocked.Exchange(ref this._Timer, null)?.Dispose();
            }
        }
    }
}
