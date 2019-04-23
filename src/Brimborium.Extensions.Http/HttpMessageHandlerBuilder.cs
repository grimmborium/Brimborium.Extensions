namespace Brimborium.Extensions.Http {
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using Microsoft.Extensions.Logging;

    /// <summary>Builder for the HttpMessageHandle - stack.</summary>
    public class HttpMessageHandlerBuilder : IHttpMessageHandlerBuilder {
        /// <summary>ctor</summary>
        /// <param name="services">the services</param>
        public HttpMessageHandlerBuilder(IServiceProvider services) {
            this.Services = services;
            this.AdditionalHandlers = new List<DelegatingHandler>();
        }

        /// <inheritdoc/>
        public IServiceProvider Services { get; }

        /// <inheritdoc/>
        public HttpClientConfiguration Configuration { get; private set; }

        /// <inheritdoc/>
        public HttpClientHandler PrimaryHandler { get; set; }

        /// <inheritdoc/>
        public List<DelegatingHandler> AdditionalHandlers { get; }

        /// <inheritdoc/>
        public HttpClientHandler EnsurePrimaryHandler() {
            var primaryHandler = this.PrimaryHandler;
            if (primaryHandler == null) {
                primaryHandler = new HttpClientHandler();
                this.PrimaryHandler = primaryHandler;
            }
            return primaryHandler;
        }

        /// <inheritdoc/>
        public void SetConfiguration(HttpClientConfiguration configuration) {
            this.Configuration = configuration;
        }

        /// <inheritdoc/>
        public virtual void ApplyConfig() {
            foreach (var action in this.Configuration.PrimaryHandlerConfigurations) {
                if (action != null) {
                    action(this);
                }
            }
            foreach (var action in this.Configuration.AdditionalHandlerConfigurations) {
                if (action != null) {
                    action(this);
                }
            }
        }

        /// <inheritdoc/>
        public virtual HttpMessageHandler Build(ILogger logger) {
            var primaryHandler = this.EnsurePrimaryHandler();
                        
            HttpMessageHandler next = new HttpMessageHandlerLogging(logger, primaryHandler);
            var additionalHandlers = this.AdditionalHandlers.ToArray();

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
