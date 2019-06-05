namespace Brimborium.Extensions.RequestPipe {
    using System.Threading.Tasks;

    public interface IRequestHandler<TRequest, TResponce>
        where TRequest : IRequest<TResponce> {
        Task<TResponce> ExecuteAsync(TRequest request);
    }
}
