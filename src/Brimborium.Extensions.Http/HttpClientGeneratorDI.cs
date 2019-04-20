namespace Microsoft.Extensions.DependencyInjection {
    using Brimborium.Extensions.Http;

    using Microsoft.Extensions.DependencyInjection.Extensions;

    public static class HttpClientGeneratorDI {
        public static void AddHttpClientGenerator(this IServiceCollection services) {
            services.AddOptions<HttpClientGeneratorOptions>();
            services.TryAddTransient<IHttpMessageHandlerBuilder, HttpMessageHandlerBuilder>();
            services.TryAddSingleton<IHttpClientGenerator, HttpClientGenerator>();
        }
    }
}
