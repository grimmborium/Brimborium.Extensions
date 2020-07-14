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
            if (request is null) {
                return default;
            }
            {
                var handler = this._ServiceProvider.GetService<IRequestHandler<IRequest<TResponce>, TResponce>>();
                if (handler is object) {
                    return handler;
                }
            }
#if
            {
                var requestType = request.GetType();
                var interfacesOfRequest = requestType.GetInterfaces();
                foreach (var interfaceOfRequest in interfacesOfRequest) {
                    if (interfaceOfRequest.IsGenericType && !interfaceOfRequest.IsGenericTypeDefinition) {
                        if (interfaceOfRequest.GetGenericTypeDefinition() == typeof(IRequest<>)) {
                            var genericArguments = interfaceOfRequest.GetGenericArguments();
                            if (genericArguments[0] == typeof(TResponce)) {
                                //var typeRequest = typeof(IRequest<>).MakeGenericType(typeof(TResponce));
                                var typeIRequestHandler = typeof(IRequestHandler<,>)
                                    .MakeGenericType(
                                        requestType, //typeRequest,
                                        typeof(TResponce));
                                var service = this._ServiceProvider.GetService(typeIRequestHandler);
                                if (service is IRequestHandler<IRequest<TResponce>, TResponce> handler) {
                                    return handler;
                                }
                            }
                        }
                    }
                    //this._ServiceProvider.GetService
                }
                //throw new NotImplementedException();
            }
#endif
            {
                return default;
            }
        }
    }
}
