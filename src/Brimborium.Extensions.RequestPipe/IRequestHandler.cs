namespace Brimborium.Extensions.RequestPipe {
    using System.Threading.Tasks;

    public interface IRequestHandlerBase {
    }

    public interface IRequestHandlerWithOptions {
        void SetOptions(IRequestHandlerOptions options);
    }

    // should be implemented by a class
    public interface IRequestHandler<in TRequest, TResponce>
        : IRequestHandlerBase
        where TRequest : IRequest<TResponce> {
        Task<TResponce> ExecuteAsync(TRequest request);
    }
}
