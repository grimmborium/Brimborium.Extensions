namespace Brimborium.Extensions.RequestPipe {
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Options;

    using System;
    using System.Threading.Tasks;

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
}
