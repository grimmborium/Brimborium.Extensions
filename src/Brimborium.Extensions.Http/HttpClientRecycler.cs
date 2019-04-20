namespace Brimborium.Extensions.Http {
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    public class HttpClientRecycler {
        private HttpClientGenerator _HttpClientGenerator;
        private ReuseRecycleHandler _ReuseRecycleHandler;
        private readonly HttpClientConfiguration _Configuration;

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

        public ReuseRecycleHandler CreateReuseRecycleHandler() {
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

            var handler = new ReuseRecycleHandler(
                messageHandler,
                this._Configuration,
                scope);

            return handler;
        }

        internal void OnDispose(DisposeRecycleHandler disposeRecycleHandler) {
            
            throw new NotImplementedException();
        }
    }

    public class HttpClientConfigurationHandler : DelegatingHandler {
        private readonly HttpClientConfiguration _Configuration;

        public HttpClientConfigurationHandler(HttpClientConfiguration configuration) {
            this._Configuration = configuration;
        }
    }

    public class ReuseRecycleHandler : DelegatingHandler {
        private readonly HttpClientConfiguration _Configuration;
        private readonly IServiceScope _Scope;

        public ReuseRecycleHandler(
            HttpMessageHandler innerHandler,
            HttpClientConfiguration configuration,
            IServiceScope scope
            ) : base(innerHandler) {
            this._Configuration = configuration;
            this._Scope = scope;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            return base.SendAsync(request, cancellationToken);
        }

        protected override void Dispose(bool disposing) {
            //base.Dispose(disposing);
        }
    }

    public class DisposeRecycleHandler : DelegatingHandler {
        private readonly HttpClientRecycler _HttpClientRecycler;

        public DisposeRecycleHandler(
            HttpMessageHandler innerHandler,
            HttpClientRecycler httpClientRecycler
            ) : base(innerHandler) {
            this._HttpClientRecycler = httpClientRecycler;
        }
        
        protected override void Dispose(bool disposing) {
            this._HttpClientRecycler.OnDispose(this);
            base.Dispose(disposing);
        }
    }
}
