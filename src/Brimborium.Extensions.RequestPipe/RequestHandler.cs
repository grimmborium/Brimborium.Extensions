namespace Brimborium.Extensions.RequestPipe {
    using System.Threading.Tasks;

    public abstract class RequestHandler<TRequest, TResponce>
        : IRequestHandler<TRequest, TResponce>, IRequestHandlerWithOptions
        where TRequest : IRequest<TResponce> {
        protected IRequestHandlerOptions Options;

        public RequestHandler() {
        }

        public void SetOptions(IRequestHandlerOptions options) {
            this.Options = options;
        }

        public virtual async Task<TResponce> ExecuteAsync(TRequest request) {
            var options = this.Options ?? RequestHandlerFallbackOptions.GetInstance();
            Task<TResponce> task;
            try {
                task = this.HandleAsync(request);
            } catch (System.Exception error) {
                options.LogError?.Invoke(error);
                throw;
            }
            try {
                if (task is null) {
                    return default(TResponce);
                } else {
                    var result = await task.ConfigureAwait(true);
                    return result;
                }
            } catch (System.Exception error) {
                options.LogError?.Invoke(error);
                throw;
            }
        }

        protected abstract Task<TResponce> HandleAsync(TRequest request);
    }
}
