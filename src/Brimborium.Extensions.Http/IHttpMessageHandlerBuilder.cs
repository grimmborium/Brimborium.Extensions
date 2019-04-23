namespace Brimborium.Extensions.Http {
    using System;
    using System.Collections.Generic;
    using System.Net.Http;

    using Microsoft.Extensions.Logging;

    /// <summary>Builder of the HttpMessageHandler stack.</summary>
    public interface IHttpMessageHandlerBuilder {
        /// <summary>
        /// Set the configuration that should be used by this builder.
        /// This is called by the <see cref="HttpClientGenerator"/>.
        /// </summary>
        /// <param name="configuration"></param>
        void SetConfiguration(HttpClientConfiguration configuration);

        /// <summary>The configuration set by <see cref="SetConfiguration"/>.</summary>
        HttpClientConfiguration Configuration { get; }

        /// <summary>The services of this.</summary>
        IServiceProvider Services { get; }

        /// <summary>The Primary Handler noramlly a Http​Client​Handler.</summary>
        HttpClientHandler PrimaryHandler { get; set; }

        /// <summary>Get the current <see cref="PrimaryHandler"/> or a new <see cref="HttpClientHandler"/>.</summary>
        /// <returns>a old or new instance - not null.</returns>
        HttpClientHandler EnsurePrimaryHandler();

        /// <summary>A list of DelegatingHandlers that build the stack.</summary>
        List<DelegatingHandler> AdditionalHandlers { get; }

        /// <summary>
        /// Calls the configurations <see cref="HttpClientConfiguration.PrimaryHandlerConfigurations"/> to configure the PrimaryHandler
        /// and <see cref="HttpClientConfiguration.AdditionalHandlerConfigurations"/> to fill/configure the AdditionalHandlers.
        /// This is called by the <see cref="HttpClientGenerator"/>.
        /// </summary>
        void ApplyConfig();

        /// <summary>
        /// Create a new HttpMessageHandler stack.
        /// This is called by the <see cref="HttpClientGenerator"/>.
        /// </summary>
        /// <param name="logger">the logger to use.</param>
        /// <returns>a HttpMessageHandler.</returns>
        HttpMessageHandler Build(ILogger logger);
    }
}
