namespace Brimborium.Extensions.Http {
    using System;
    using System.Collections.Generic;
    using System.Net.Http;

    public interface IHttpMessageHandlerBuilder {
        void SetConfiguration(HttpClientConfiguration configuration);

        IServiceProvider Services { get; }

        HttpClientConfiguration Configuration { get; }

        HttpMessageHandler PrimaryHandler { get; set; }

        List<DelegatingHandler> AdditionalHandlers { get; }

        void ApplyConfig();

        HttpMessageHandler Build();
    }
}
