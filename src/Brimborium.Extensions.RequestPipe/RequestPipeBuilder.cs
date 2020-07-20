namespace Brimborium.Extensions.RequestPipe {
    using System;
    using System.Linq;
    using System.Reflection;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public class RequestPipeBuilder {
        private Action<ServiceDescriptor> _ServicesAdd;

        public RequestPipeBuilder(IServiceCollection services) {
            this.Services = services;
            this._ServicesAdd = this.ServicesAddDefault;
            this.ServiceLifetimeOfRequestExecutionService = ServiceLifetime.Scoped;
            this.ServiceLifetimeOfRequestHandler = ServiceLifetime.Transient;
            this.RequestExecutionServiceType = typeof(RequestExecutionService);
            this.RequestHandlerSolverType = typeof(RequestHandlerSolver);
        }

        private void ServicesAddDefault(ServiceDescriptor serviceDescriptor) {
            this.Services.Add(serviceDescriptor);
        }

        public IServiceCollection Services { get; }
        public ServiceLifetime ServiceLifetimeOfRequestExecutionService { get; }
        public ServiceLifetime ServiceLifetimeOfRequestHandler { get; }
        public Action<ServiceDescriptor> ServicesAdd {
            get {
                return _ServicesAdd;
            }
            set {
                _ServicesAdd = value ?? this.ServicesAddDefault;
            }
        }
        public Type RequestExecutionServiceType { get; private set; }
        public Type RequestHandlerSolverType { get; private set; }

        public RequestPipeBuilder SetRequestExecutionServiceType<TRequestExecutionService>()
            where TRequestExecutionService : IRequestExecutionService {
            this.RequestExecutionServiceType = typeof(TRequestExecutionService);
            return this;
        }

        public RequestPipeBuilder SetRequestHandlerSolverType<TRequestHandlerSolver>()
            where TRequestHandlerSolver : IRequestHandlerSolver {
            this.RequestHandlerSolverType = typeof(TRequestHandlerSolver);
            return this;
        }

        public RequestPipeBuilder AddRequestHandler<TRequestHandlerBase>()
            where TRequestHandlerBase : IRequestHandlerBase {
            var arrInterfaces = typeof(TRequestHandlerBase).GetInterfaces();
            bool success = false;
            foreach (Type tinterface in arrInterfaces) {
                if (tinterface.IsGenericType && !tinterface.IsGenericTypeDefinition) {
                    var genericTypeDefinition = tinterface.GetGenericTypeDefinition();
                    if (genericTypeDefinition == typeof(IRequestHandler<,>)) {
                        success = true;
                        this.AddRequestHandler(
                            tinterface,
                            typeof(TRequestHandlerBase),
                            this.ServiceLifetimeOfRequestHandler);
                        //var a = tinterface.GetGenericArguments();
                        // a[0]Request
                        // a[1]Responce
                        return this;
                    }
                }
            }
            if (success) {
                return this;
            } else {
                throw new ArgumentException("IRequestHandler<,> not found.");
            }
        }

      
        public RequestPipeBuilder AddRequestHandlers(params Type[] typeRequestHandlers) {
            foreach (var typeRequestHandler in typeRequestHandlers) {
                if (typeRequestHandler is null) {
                    continue;
                }
                {
                    var arrInterfaces = typeRequestHandler.GetInterfaces();
                    bool success = false;
                    foreach (Type tinterface in arrInterfaces) {
                        if (tinterface.IsGenericType && !tinterface.IsGenericTypeDefinition) {
                            var genericTypeDefinition = tinterface.GetGenericTypeDefinition();
                            if (genericTypeDefinition == typeof(IRequestHandler<,>)) {
                                success = true;
                                //var a = tinterface.GetGenericArguments();
                                // a[0]Request
                                // a[1]Responce
                                this.AddRequestHandler(
                                        tinterface,
                                        typeRequestHandler,
                                        this.ServiceLifetimeOfRequestHandler);
                                return this;
                            }
                        }
                    }

                    if (success) {
                        //
                    } else {
                        throw new ArgumentException("IRequestHandler<,> not found.");
                    }
                }
            }
            return this;
        }

        public void Add<TRequest, TResponse>(
            Func<IServiceProvider, IRequestHandler<TRequest, TResponse>> requestHandlerFunc
            )
            where TRequest : IRequest<TResponse> {
            this.ServicesAdd(new ServiceDescriptor(
                typeof(IRequestHandler<TRequest, TResponse>),
                requestHandlerFunc,
                this.ServiceLifetimeOfRequestHandler
                ));
        }

        private void AddRequestHandler(Type tInterface, Type tService, ServiceLifetime serviceLifetime) {
            this.ServicesAdd(
                new ServiceDescriptor(
                    tInterface,
                    tService,
                    this.ServiceLifetimeOfRequestHandler));
        }

        public RequestPipeBuilder AddRequestHandlersOfAssembly(params Assembly[] assemblies) {
            // TODO
            return this;
        }
        public RequestPipeBuilder AddRequestHandlersOfAssembly(params Type[] typeRequestHandlers) {
            this.AddRequestHandlersOfAssembly(typeRequestHandlers.Select(t => t.Assembly).Distinct().ToArray());
            return this;
        }

        internal void Build() {
            this.Services.TryAdd(
                new ServiceDescriptor(
                    typeof(IRequestExecutionService),
                    this.RequestExecutionServiceType,
                    ServiceLifetime.Scoped
                    ));
            this.Services.TryAdd(
                new ServiceDescriptor(
                    typeof(IRequestHandlerSolver),
                    this.RequestHandlerSolverType,
                    ServiceLifetime.Scoped
                    ));
        }
    }
}
