namespace Brimborium.Extensions.RequestPipe {
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;

    public class RequestExecutionService
        : IRequestExecutionService {
        protected readonly IOptions<RequestPipeOptions> _Options;
        protected readonly IServiceProvider _ServiceProvider;
        protected IRequestHandlerSolver _Solver;

        public RequestExecutionService(IOptions<RequestPipeOptions> options, IServiceProvider serviceProvider) {
            //IOptionsMonitor<TOptions> x;
            this._Options = options ?? throw new ArgumentNullException(nameof(options));
            this._ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public virtual async Task<TResponce> ExecuteAsync<TResponce>(IRequest<TResponce> request) {
            if (request is null) { throw new ArgumentNullException(nameof(request)); }
            var handler = this.GetSolver().Solve<TResponce>(request);
            var task = handler.ExecuteAsync(request);
            var result = await task.ConfigureAwait(false);
            return result;
        }

        protected virtual IRequestHandlerSolver GetSolver() {
            var solver = this._Solver;
            if (solver is null) {
                solver = this._Options?.Value?.Solver;
                if (solver is null) {
                    solver = this._ServiceProvider.GetRequiredService<IRequestHandlerSolver>();
                }
                this._Solver = solver;
            }
            return solver;
        }
    }

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
