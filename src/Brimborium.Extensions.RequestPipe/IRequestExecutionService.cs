namespace Brimborium.Extensions.RequestPipe {
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;

    public interface IRequestExecutionService {
        Task<Response<TResponse>> ExecuteAsync<TResponse>(
            IRequest<TResponse> request,
            System.Threading.CancellationToken cancellationToken,
            IRequestHandlerExecutionContext executionContext);

        Task<Response<object?>> ExecuteAsync(
            IRequestBase request,
            System.Threading.CancellationToken cancellationToken,
            IRequestHandlerExecutionContext executionContext);
    }


    public interface IRequestHandlerTranslate<TResponse> {
        Task<Response<TResponse>> ExecuteAnyAsync(
            object request,
            CancellationToken cancellationToken,
            IRequestExecutionServiceInner requestExecutionService);
        Task<Response<TResponse>> ExecuteTypedAsync(
            IRequest<TResponse> request,
            CancellationToken cancellationToken,
            IRequestExecutionServiceInner requestExecutionService);
    }

    public interface IRequestHandlerFactory {
        T CreateRequestHandler<T>() where T:IRequestHandlerBase;

        IEnumerable<T>? CreateRequestHandlerChains<T>() where T:IRequestHandlerChainBase;
    }

    public interface IRequestExecutionServiceInner : IRequestExecutionService {
        IRequestHandlerFactory GetRequestHandlerFactory();
        List<RequestPipeOptions> GetRequestPipeOptions<TRequest, TResponse>() where TRequest : IRequest<TResponse>;
    }

    public interface IRequestHandlerSolver {
        IRootRequestHandler<TResponse> GetRequestHandler<TResponse>(IRequest<TResponse> request, IRequestHandlerExecutionContext executionContext);
        IRootRequestHandler<TResponse> GetRootRequestHandler<TRequest, TResponse>() where TRequest : IRequest<TResponse>;

    }
}