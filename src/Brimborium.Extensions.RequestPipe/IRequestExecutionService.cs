namespace Brimborium.Extensions.RequestPipe {
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IRequestExecutionService {
        Task<TResponce> ExecuteAsync<TResponce>(IRequest<TResponce> request);
    }

    public interface IRequestHandlerTranslate<TResponse> {
        Task<TResponse> Handle(
            IRequest<TResponse> request,
            CancellationToken cancellationToken,
            IRequestHandlerFactory requestHandlerFactory);
    }

    public interface IRequestHandlerFactory {
        T GetInstance<T>();

        IEnumerable<T> GetInstances<T>();
    }

    public interface IRequestHandlerSolver {
        IRequestHandler<IRequest<TResponce>, TResponce> Solve<TResponce>(IRequest<TResponce> request);
    }
}