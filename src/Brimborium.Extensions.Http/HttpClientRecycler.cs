namespace Brimborium.Extensions.Http {
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Net.Http;
    using System.Threading;

    /// <summary>Controlls the creating and disposing of the HttpClient and HttpMessageHandler stack.</summary>
    public class HttpClientRecycler {
        private readonly IServiceProvider _Services;
        private readonly IServiceScopeFactory _ScopeFactory;
        private readonly ILoggerFactory _LoggerFactory;
        private ReuseRecycleHandler _ReuseRecycleHandler;
        private readonly HttpClientConfiguration _Configuration;
        private int _Usage;
        private Timer _Timer;

        /// <summary>ctor</summary>
        /// <param name="services">the services</param>
        /// <param name="scopeFactory">the scope factory</param>
        /// <param name="loggerFactory">the logger factory</param>
        /// <param name="configuration">the configuration</param>
        public HttpClientRecycler(
            IServiceProvider services,
            IServiceScopeFactory scopeFactory,
            ILoggerFactory loggerFactory,
            HttpClientConfiguration configuration
            ) {
            this._Services = services;
            this._ScopeFactory = scopeFactory;
            this._LoggerFactory = loggerFactory;
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
                var services = this._Services;
                var scope = (IServiceScope)null;

                if (!this._Configuration.SuppressHandlerScope) {
                    scope = this._ScopeFactory?.CreateScope();
                    if (scope != null) {
                        services = scope.ServiceProvider;
                    }
                }

                var loggerName = this._Configuration.Name;
                //
                var outerLogger = this._LoggerFactory.CreateLogger($"System.Net.Http.HttpClient.{loggerName}.LogicalHandler");
                var innerLogger = this._LoggerFactory.CreateLogger($"System.Net.Http.HttpClient.{loggerName}.ClientHandler");

                var builder = services.GetRequiredService<IHttpMessageHandlerBuilder>();
                builder.SetConfiguration(this._Configuration);
                builder.ApplyConfig();
                var messageHandler = builder.Build(innerLogger);
                
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
