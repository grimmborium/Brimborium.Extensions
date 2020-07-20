namespace Brimborium.Extensions.RequestPipe {
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IRequestExecutionService {
        Task<TResponse> ExecuteAsync<TResponse>(
            IRequest<TResponse> request,
            System.Threading.CancellationToken cancellationToken,
            IRequestHandlerExecutionContext executionContext);
    }

    public interface IRequestHandlerTranslate<TResponse> {
        Task<TResponse> ExecuteAnyAsync(
            object request,
            CancellationToken cancellationToken,
            IRequestExecutionServiceInner requestExecutionService);
        Task<TResponse> ExecuteTypedAsync(
            IRequest<TResponse> request,
            CancellationToken cancellationToken,
            IRequestExecutionServiceInner requestExecutionService);
    }

    public interface IRequestHandlerFactory {
        T CreateRequestHandler<T>();

        IEnumerable<T>? CreateRequestHandlerChains<T>();
    }

    public interface IRequestExecutionServiceInner : IRequestExecutionService {
        IRequestHandlerFactory GetRequestHandlerFactory();
    }

    public interface IRequestHandlerSolver {
        IRootRequestHandler<TResponse> GetRequestHandler<TResponse>(IRequest<TResponse> request, IRequestHandlerExecutionContext executionContext);
        IRootRequestHandler<TResponse> GetRootRequestHandler<TRequest, TResponse>() where TRequest : IRequest<TResponse>;
        
    }
}