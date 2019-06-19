
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
    }

    public class RequestPipeOptions {
        public IServiceCollection Services { get; set; }

        public IRequestHandlerSolver Solver { get; set; }

        public RequestPipeOptions() {
        }

        public void AddSingleton<TRequest, TResponce>(IRequestHandler<TRequest, TResponce> requestHandler)
            where TRequest : IRequest<TResponce> {
            this.Services.AddSingleton<IRequestHandler<TRequest, TResponce>>(requestHandler);
        }

        public void Add<TRequest, TResponce>(Func<IServiceProvider, IRequestHandler<TRequest, TResponce>> requestHandlerFunc)
            where TRequest : IRequest<TResponce> {
            this.Services.AddTransient<IRequestHandler<TRequest, TResponce>>(requestHandlerFunc);
        }

        public void Add<TRequest, TResponce, TRequestHandler>()
            where TRequest : IRequest<TResponce> 
            where TRequestHandler: class, IRequestHandler<TRequest, TResponce>{
            this.Services.AddTransient<IRequestHandler<TRequest, TResponce>, TRequestHandler>();
        }
    }
}
