namespace Brimborium.Extensions.RequestPipe {
    using System.Threading;
    using System.Threading.Tasks;

    public abstract class RequestHandler<TRequest, TResponse>
        : IRequestHandler<TRequest, TResponse>, IRequestHandlerWithOptions
        where TRequest : IRequest<TResponse> {
        protected IRequestHandlerOptions? Options;

        public RequestHandler() {
        }

        public void SetOptions(IRequestHandlerOptions options) {
            this.Options = options;
        }

        public virtual async Task<TResponse> ExecuteAsync(
            TRequest request,
            System.Threading.CancellationToken cancellationToken,
            IRequestHandlerExecutionContext executionContext) {
            var options = this.Options ?? RequestHandlerFallbackOptions.GetInstance();
            Task<TResponse> task;
            try {
                task = this.HandleAsync(request, cancellationToken, executionContext);
            } catch (System.Exception error) {
                options.LogError?.Invoke(error);
                throw;
            }
            try {
                if (task is null) {
                    //return default(TResponse);
                    throw new System.InvalidOperationException();
                } else {
                    var result = await task.ConfigureAwait(true);
                    return result;
                }
            } catch (System.Exception error) {
                options.LogError?.Invoke(error);
                throw;
            }
        }

        protected abstract Task<TResponse> HandleAsync(
            TRequest request,
            System.Threading.CancellationToken cancellationToken,
            IRequestHandlerExecutionContext executionContext);
    }

    public class RequestHandlerChain<TRequest, TResponse>
       : IRequestHandlerChain<TRequest, TResponse>
       where TRequest : IRequest<TResponse> {
        public Task<TResponse> ExecuteAsync(
            TRequest request,
            CancellationToken cancellationToken,
            IRequestHandlerExecutionContext executionContext,
            IRequestHandler<TRequest, TResponse> next) {
            return next.ExecuteAsync(request, cancellationToken, executionContext);
        }
    }
}
