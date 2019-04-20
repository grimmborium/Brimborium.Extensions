namespace Brimborium.Extensions.Http {
    using System;
    using System.Collections.Generic;
    using System.Net.Http;

    public class HttpMessageHandlerBuilder : IHttpMessageHandlerBuilder {

        public HttpMessageHandlerBuilder(IServiceProvider services) {
            this.Services = services;
            this.AdditionalHandlers = new List<DelegatingHandler>();
        }
        public IServiceProvider Services { get; }

        public HttpClientConfiguration Configuration { get; private set; }

        public HttpMessageHandler PrimaryHandler { get; set; }

        public List<DelegatingHandler> AdditionalHandlers { get; }

        /// <summary>Get the current <see cref="PrimaryHandler"/> or a new <see cref="HttpClientHandler"/>.</summary>
        /// <returns>a old or new instance not null.</returns>
        public HttpMessageHandler EnsurePrimaryHandler() {
            var primaryHandler = this.PrimaryHandler;
            if (primaryHandler == null) {
                primaryHandler = new HttpClientHandler();
                this.PrimaryHandler = primaryHandler;
            }
            return primaryHandler;
        }

        public void SetConfiguration(HttpClientConfiguration configuration) {
            this.Configuration = configuration;
        }

        public virtual void ApplyConfig() {
            foreach (var action in this.Configuration.PrimaryConfigure) {
                if (action != null) {
                    action(this);
                }
            }
            foreach (var action in this.Configuration.AdditionalConfigure) {
                if (action != null) {
                    action(this);
                }
            }
        }

        public virtual HttpMessageHandler Build() {
            var primaryHandler = this.EnsurePrimaryHandler();
            var additionalHandlers = this.AdditionalHandlers.ToArray();

            var next = primaryHandler;
            for (int idx = additionalHandlers.Length - 1; idx >= 0; idx--) {
                var handler = additionalHandlers[idx];
                if (handler == null) {
                    continue;
                } else {
                    if (handler.InnerHandler != null) {
                        throw new InvalidOperationException($"({handler.GetType().FullName})handler.InnerHandler is already set.");
                    } else {
                        handler.InnerHandler = next;
                        next = handler;
                    }
                }
            }

            return next;
        }
    }
}
