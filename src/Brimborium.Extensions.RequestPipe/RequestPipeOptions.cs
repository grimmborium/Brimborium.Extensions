
namespace Brimborium.Extensions.RequestPipe {
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Microsoft.Extensions.DependencyInjection;
    public class RequestPipeBuilder {
        public RequestPipeBuilder(IServiceCollection services) {
            this.Services = services;
        }

        public IServiceCollection Services { get; }


        public void AddSingleton<TRequestHandlerBase>()
            where TRequestHandlerBase : IRequestHandlerBase {
            var arrInterfaces = typeof(TRequestHandlerBase).GetInterfaces();
            foreach (Type tinterface in arrInterfaces) {
                if (tinterface.IsGenericType) {
                    var x = tinterface.GetGenericTypeDefinition();
                    if (x == typeof(IRequestHandler<,>)) {
                        var a = tinterface.GetGenericArguments();
                        // a[0]Request
                        // a[1]Responce
                    }

                }
            }

            //typeof(TRequestHandlerBase).GetInterface()
            // this.Services.Add(new ServiceDescriptor());
            //System.Collections.ObjectModel.Collection<int>
        }

        public void AddSingleton(IRequestHandlerBase requestHandler) {
            // this.Services.Add(new ServiceDescriptor());
        }


        public void AddSingleton<TRequest, TResponce>(IRequestHandler<TRequest, TResponce> requestHandler)
            where TRequest : IRequest<TResponce> {
            this.Services.AddSingleton<IRequestHandler<TRequest, TResponce>>(requestHandler);
        }

        public void AddScoped<TRequest, TResponce>(Func<IServiceProvider, IRequestHandler<TRequest, TResponce>> requestHandlerFunc)
            where TRequest : IRequest<TResponce> {
            this.Services.AddScoped<IRequestHandler<TRequest, TResponce>>(requestHandlerFunc);
        }

        public void AddScoped<TRequest, TResponce, TRequestHandler>()
            where TRequest : IRequest<TResponce>
            where TRequestHandler : class, IRequestHandler<TRequest, TResponce> {
            this.Services.AddTransient<IRequestHandler<TRequest, TResponce>, TRequestHandler>();
        }

        public void AddTransient<TRequest, TResponce>(Func<IServiceProvider, IRequestHandler<TRequest, TResponce>> requestHandlerFunc)
                where TRequest : IRequest<TResponce> {
            this.Services.AddTransient<IRequestHandler<TRequest, TResponce>>(requestHandlerFunc);
        }

        public void AddTransient<TRequest, TResponce, TRequestHandler>()
            where TRequest : IRequest<TResponce>
            where TRequestHandler : class, IRequestHandler<TRequest, TResponce> {
            this.Services.AddTransient<IRequestHandler<TRequest, TResponce>, TRequestHandler>();
        }
    }

    public class RequestPipeOptions {
        public IServiceCollection Services { get; set; }

        public IRequestHandlerSolver Solver { get; set; }

        public RequestPipeOptions() {
        }

    }
}
