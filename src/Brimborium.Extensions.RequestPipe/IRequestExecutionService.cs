namespace Brimborium.Extensions.RequestPipe {
    using System.Threading.Tasks;

    public interface IRequestExecutionService {
        Task<TResponce> ExecuteAsync<TResponce>(IRequest<TResponce> request);
    }

    public interface IRequestHandlerSolver {
        IRequestHandler<IRequest<TResponce>, TResponce> Solve<TResponce>(IRequest<TResponce> request);
    }
}