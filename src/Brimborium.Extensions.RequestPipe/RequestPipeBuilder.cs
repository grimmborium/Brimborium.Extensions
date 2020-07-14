
namespace Brimborium.Extensions.RequestPipe {
    using System;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public class RequestPipeBuilder {
        public RequestPipeBuilder(IServiceCollection services) {
            this.Services = services;
        }

        public IServiceCollection Services { get; }


        public void Add<TRequestHandlerBase>()
            where TRequestHandlerBase : IRequestHandlerBase {
            var arrInterfaces = typeof(TRequestHandlerBase).GetInterfaces();
            foreach (Type tinterface in arrInterfaces) {
                if (tinterface.IsGenericType) {
                    var genericTypeDefinition = tinterface.GetGenericTypeDefinition();
                    if (genericTypeDefinition == typeof(IRequestHandler<,>)) {
                        //var a = tinterface.GetGenericArguments();
                        // a[0]Request
                        // a[1]Responce
                        this.Services.Add(new ServiceDescriptor(tinterface, typeof(TRequestHandlerBase), ServiceLifetime.Transient));
                        return;
                    }
                }
            }
            throw new ArgumentException("IRequestHandler<,> not found.");
        }
        
        public void Add<TRequest, TResponce>(Func<IServiceProvider, IRequestHandler<TRequest, TResponce>> requestHandlerFunc)
            where TRequest : IRequest<TResponce> {
            this.Services.AddTransient<IRequestHandler<TRequest, TResponce>>(requestHandlerFunc);
        }

        public void Add<TRequest, TResponce, TRequestHandler>()
            where TRequest : IRequest<TResponce>
            where TRequestHandler : class, IRequestHandler<TRequest, TResponce> {
            //where TRequestHandler : class, IRequestHandler<IRequest<TResponce>, TResponce> {
            
            this.Services.TryAddTransient<IRequestHandler<TRequest, TResponce>, TRequestHandler>();
            //this.Services.TryAddTransient<IRequestHandler<IRequest<TResponce>, TResponce>, TRequestHandler>();
            //var typeIRequestOfTResponce = typeof(IRequest<>).MakeGenericType(typeof(TResponce));
            //if (typeIRequestOfTResponce == typeof(TRequest)) {
            //    // nothing todo
            //} else {
            //    var typeIRequestHandler = typeof(IRequestHandler<,>).MakeGenericType(typeIRequestOfTResponce, typeof(TResponce));
            //    if (typeIRequestHandler != typeof(TRequestHandler)) { 
            //        this.Services.TryAddTransient(typeIRequestHandler, typeof(TRequestHandler));
            //    }
            //}
        }
    }
}
