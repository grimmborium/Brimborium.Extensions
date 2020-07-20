namespace Brimborium.Extensions.RequestPipe {
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;

    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public class RequestExecutionService
        : IRequestExecutionService
        , IRequestExecutionServiceInner {
        protected readonly RequestPipeOptions _Options;
        protected readonly IRequestHandlerFactory _RequestHandlerFactory;
        protected IRequestHandlerSolver? _Solver;

        public RequestExecutionService(RequestPipeOptions? options, IRequestHandlerFactory requestHandlerFactory) {
            options ??= new RequestPipeOptions();
            this._Options = options;
            this._RequestHandlerFactory = requestHandlerFactory ?? options.RequestHandlerFactory ?? throw new ArgumentNullException(nameof(options));
        }

        public RequestExecutionService(RequestPipeOptions options, IServiceProvider serviceProvider)
            : this(options, new RequestHandlerFactoryWithServiceProvider(serviceProvider)) {
        }


        [Microsoft.Extensions.DependencyInjection.ActivatorUtilitiesConstructor]
        public RequestExecutionService(IOptions<RequestPipeOptions> options, IServiceProvider serviceProvider)
            : this(options?.Value, new RequestHandlerFactoryWithServiceProvider(serviceProvider)) {
        }

        public virtual Task<Response<TResponse>> ExecuteAsync<TResponse>(
            IRequest<TResponse> request,
            System.Threading.CancellationToken cancellationToken) {
            var executionContext = new RequestHandlerExecutionContext();
            return this.ExecuteAsync(request, cancellationToken, executionContext);
        }

        public virtual async Task<Response<TResponse>> ExecuteAsync<TResponse>(
            IRequest<TResponse> request,
            System.Threading.CancellationToken cancellationToken,
            IRequestHandlerExecutionContext executionContext) {
            if (request is null) { throw new ArgumentNullException(nameof(request)); }
            if (executionContext is null) {
                executionContext = new RequestHandlerExecutionContext();
            }
            var solver = this.GetSolver();

            var handler = solver.GetRequestHandler<TResponse>(request, executionContext);
            var task = handler.ExecuteTypedAsync(request, cancellationToken, executionContext);
            var result = await task.ConfigureAwait(false);
            return result;
        }
       
        public virtual async Task<Response<object?>> ExecuteAsync(
            IRequestBase request,
            System.Threading.CancellationToken cancellationToken,
            IRequestHandlerExecutionContext executionContext) {
            // request.GetType();
            await Task.CompletedTask;
            throw new Exception();
        }

        public IRequestHandlerFactory GetRequestHandlerFactory() => this._RequestHandlerFactory;
        public List<RequestPipeOptions> GetRequestPipeOptions<TRequest, TResponse>() where TRequest : IRequest<TResponse> => throw new NotImplementedException();

        protected virtual IRequestHandlerSolver GetSolver() {
            var solver = this._Solver;
            if (solver is object) {
                return solver;
            }

            {
                solver = this._Options.Solver;
                if (solver is null) {
                    solver = new RequestHandlerSolver(this._RequestHandlerFactory);
                }
                if (System.Threading.Interlocked.CompareExchange(ref this._Solver, solver, null) is null) {
                    System.Threading.Interlocked.MemoryBarrier();
                    return solver;
                } else {
                    return this._Solver;
                }
            }
        }
    }
}
