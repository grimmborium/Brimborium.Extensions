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
}
