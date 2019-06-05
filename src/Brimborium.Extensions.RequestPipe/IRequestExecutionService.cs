namespace Brimborium.Extensions.RequestPipe {
    using System.Threading.Tasks;

    public interface IRequestExecutionService {
        Task<TResponce> ExecuteAsync<TResponce>(IRequest<TResponce> request);
    }
}