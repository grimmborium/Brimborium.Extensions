namespace Brimborium.Extensions.RequestPipe {
    using System;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public class RequestHandlerSolver : IRequestHandlerSolver {
        private readonly IServiceProvider _ServiceProvider;

        public RequestHandlerSolver(IServiceProvider serviceProvider) {
            this._ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public virtual IRequestHandler<IRequest<TResponce>, TResponce> Solve<TResponce>(IRequest<TResponce> request) {
            var handler = this._ServiceProvider.GetService<IRequestHandler<IRequest<TResponce>, TResponce>>();
            if (handler is null) {
                var requestType = request.GetType();
                var interfaces = requestType.GetInterfaces();
            }
            return handler;
        }
    }
}
