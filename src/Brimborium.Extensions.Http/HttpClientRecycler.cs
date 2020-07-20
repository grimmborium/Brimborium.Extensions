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
            System.Threading.Interlocked.Exchange(ref this._Timer, null)?.Dispose();

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
            // using ref count instead of weak references
            System.Threading.Interlocked.Increment(ref this._Usage);
            try {
                //
                var trigger = new DisposeRecycleHandler(handler, this);
                var httpClient = new HttpClient(trigger, true);
                if (!string.IsNullOrEmpty(this._Configuration.BaseAddress)) {
                    httpClient.BaseAddress = new Uri(this._Configuration.BaseAddress);
                }
                foreach (var action in this._Configuration.HttpClientConfigurations) {
                    if (action != null) {
                        action(httpClient, this._Configuration);
                    }
                }
                return httpClient;
            } catch {
                System.Threading.Interlocked.Decrement(ref this._Usage);
                throw;
            }
        }

        private ReuseRecycleHandler CreateReuseRecycleHandler() {
            // within lock
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
            var outerLogger = this._LoggerFactory?.CreateLogger($"System.Net.Http.HttpClient.{loggerName}.LogicalHandler");
            var innerLogger = this._LoggerFactory?.CreateLogger($"System.Net.Http.HttpClient.{loggerName}.ClientHandler");

            var builder = services.GetRequiredService<IHttpMessageHandlerBuilder>();
            builder.SetConfiguration(this._Configuration);
            builder.ApplyConfig();
            var messageHandler = builder.Build(innerLogger);
                
            var handler = new ReuseRecycleHandler(
                messageHandler,
                scope,
                outerLogger
                );

            System.Threading.Interlocked.Exchange(ref this._Timer, null)?.Dispose();

            return handler;
        }

        internal bool IsValid() {
            System.Threading.Interlocked.Exchange(ref this._Timer, null)?.Dispose();
            var nextInnerHandler = this._ReuseRecycleHandler.InnerHandler;
            var innerHandler = nextInnerHandler;
            while (innerHandler is object) {
                nextInnerHandler = innerHandler;
                if (nextInnerHandler is DelegatingHandler delegatingHandler) {
                    innerHandler = delegatingHandler.InnerHandler;
                }
            }
            if (innerHandler is HttpClientHandlerEx httpClientHandler) {
                return !(httpClientHandler.IsDisposed);
            }
            return true;
        }

        internal void OnDispose(DisposeRecycleHandler disposeRecycleHandler) {
            if (System.Threading.Interlocked.Decrement(ref this._Usage) <= 0) {
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

        internal IDisposable GetUsageLock() {
            System.Threading.Interlocked.Increment(ref this._Usage);
            return new UsageLock(this);
        }

        class UsageLock : IDisposable {
            private HttpClientRecycler _Owner;

            public UsageLock(HttpClientRecycler owner) {
                this._Owner = owner;
            }
            protected virtual void Dispose(bool disposing) {
                var owner = System.Threading.Interlocked.Exchange(ref this._Owner, null);
                if (owner is object) {
                    owner.OnDispose(null);
                }
            }

            ~UsageLock() {
                Dispose(disposing: false);
            }

            public void Dispose() {
                System.GC.SuppressFinalize(this);
                this.Dispose(disposing: true);
            }
        }

    }
    public class HttpClientHandlerEx : HttpClientHandler {
        public bool IsDisposed { get; private set; }
        public HttpClientHandlerEx() :base(){
        }
        protected override void Dispose(bool disposing) {
            this.IsDisposed = true;
            base.Dispose(disposing);
        }
    }
}
