namespace Brimborium.Extensions.RequestPipe {
    using System.Threading.Tasks;

    public interface IRequestHandlerBase {
    }

    // should be implemented by a class
    public interface IRequestHandler<TRequest, TResponce>
        : IRequestHandlerBase
        where TRequest : IRequest<TResponce> {
        Task<TResponce> ExecuteAsync(TRequest request);
    }

    public class RequestHandler<TRequest, TResponce>
        : IRequestHandler<TRequest, TResponce>
        where TRequest : IRequest<TResponce> {

        public RequestHandler() {
        }

        public virtual async Task<TResponce> ExecuteAsync(TRequest request) {
            Task<TResponce> task;
            try {
                task = this.HandleAsync(request);
            } catch (System.Exception) {
                throw;
            }
            try {
                if (task is null) {
                    return default(TResponce);
                } else {
                    var result = await task.ConfigureAwait(true);
                    return result;
                }
            } catch (System.Exception) {
                throw;
            }
        }

        protected virtual Task<TResponce> HandleAsync(TRequest request)
            => null;
    }
}
