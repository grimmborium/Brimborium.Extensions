namespace Brimborium.Extensions.RequestPipe {
    using Microsoft.Extensions.DependencyInjection;

    using System;
    using System.Collections.Generic;

    public class RequestPipeOptions {

        public IRequestHandlerFactory? RequestHandlerFactory { get; set; }
        public IRequestHandlerSolver? Solver { get; set; }

        //public List<Func<IServiceProvider, IRequestHandlerBase>> Generators; 

        public RequestPipeOptions() {
            //this.Generators = new List<Func<IServiceProvider, IRequestHandlerBase>>();
        }

        public RequestPipeOptions AddHandler<TRequest, TResponse>(
            Func<TRequest, bool> condition)
            where TRequest:IRequest<TResponse>
            {

            return this;
        }
    }

    public class xx { 
    }

    //public class GeneratorDefinition {
    //    public Type? InterfaceRequest;
    //    public Type? InterfaceResponce;
    //    public Func<object>? Generator;
    //}

}
