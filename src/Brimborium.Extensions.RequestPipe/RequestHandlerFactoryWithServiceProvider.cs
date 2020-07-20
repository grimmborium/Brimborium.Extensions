namespace Brimborium.Extensions.RequestPipe {
    using Microsoft.Extensions.DependencyInjection;

    using System;
    using System.Collections.Generic;

    public class RequestHandlerFactoryWithServiceProvider : IRequestHandlerFactory {
        private readonly IServiceProvider _ServiceProvider;

        public RequestHandlerFactoryWithServiceProvider(IServiceProvider serviceProvider) {
            this._ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public T CreateRequestHandler<T>() where T : IRequestHandlerBase => this._ServiceProvider.GetRequiredService<T>();

        public IEnumerable<T>? CreateRequestHandlerChains<T>()
            where T : IRequestHandlerChainBase {
            var result = this._ServiceProvider.GetServices<T>();
            if (result is null) {
                return Array.Empty<T>();
            } else {
                return result;
            }
        }
    }
}
