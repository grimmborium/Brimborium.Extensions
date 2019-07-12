namespace Brimborium.Extensions.RequestPipe {
    using Microsoft.Extensions.DependencyInjection;

    using System;
    using System.Collections.Generic;

    public class RequestPipeOptions {
        public IServiceCollection Services { get; set; }

        public IRequestHandlerSolver Solver { get; set; }

        public List<Func<IServiceProvider, IRequestHandlerBase>> Generators; 

        public RequestPipeOptions() {
            this.Generators = new List<Func<IServiceProvider, IRequestHandlerBase>>();
        }
    }

    public class GeneratorDefinition {
        public Type InterfaceRequest;
        public Type InterfaceResponce;
        public Func<object> Generator;
    }

}
