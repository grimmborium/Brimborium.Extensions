namespace Microsoft.Extensions.DependencyInjection {
    using System;
    using Brimborium.Extensions.Http;

    using Microsoft.Extensions.DependencyInjection.Extensions;

    public static class HttpClientGeneratorExtensions {
        public static void AddHttpClientGenerator(this IServiceCollection services, Action<HttpClientGeneratorOptions> configure = null) {
            var optionBuilder = services.AddOptions<HttpClientGeneratorOptions>();
            if (configure != null) {
                optionBuilder.Configure(configure);
            }
            services.TryAddTransient<IHttpMessageHandlerBuilder, HttpMessageHandlerBuilder>();
            services.TryAddSingleton<IHttpClientGenerator, HttpClientGenerator>();
        }
    }
}
