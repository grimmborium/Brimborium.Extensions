using System.Threading.Tasks;

using Brimborium.Extensions.Decoration;

namespace Brimborium.Extensions.RequestPipe {
    public interface IRequestHandlerBase {
    }

    public interface IRequestHandlerWithOptions {
        void SetOptions(IRequestHandlerOptions options);
    }
    public interface IRequestHandlerChainBase {
        int GetOrder();
    }

    public interface IRequestHandlerExecutionContext { }

    // should be implemented by a class
    public interface IRequestHandler<in TRequest, TResponse>
        : IRequestHandlerBase
        where TRequest : IRequest<TResponse> {
        Task<Response<TResponse>> ExecuteAsync(
            TRequest request,
            System.Threading.CancellationToken cancellationToken,
            IRequestHandlerExecutionContext executionContext);
    }

    public interface IRootRequestHandler {
        Task<Response<object?>> ExecuteObjectAsync(
            object request,
            System.Threading.CancellationToken cancellationToken,
            IRequestHandlerExecutionContext executionContext);
    }

    public interface IRootRequestHandler<TResponse>
        : IRootRequestHandler {
        Task<Response<TResponse>> ExecuteTypedAsync(
            IRequest<TResponse> request,
            System.Threading.CancellationToken cancellationToken,
            IRequestHandlerExecutionContext executionContext);
    }

    // public delegate Task<Response<TResponse>> RequestHandlerChainDelegate<in TRequest, TResponse>(TRequest request, IRequestHandlerExecutionContext executionContext);

    public interface IRequestHandlerChain<TRequest, TResponse>
        : IRequestHandlerChainBase
        where TRequest : IRequest<TResponse> {

        Task<Response<TResponse>> ExecuteAsync(
            TRequest request,
            System.Threading.CancellationToken cancellationToken,
            IRequestHandlerExecutionContext executionContext,
            IRequestHandler<TRequest, TResponse> next);
    }

    public class RequestHandlerConfiguration {
        public RequestHandlerConfiguration() {
        }

        public System.Type? RequestHandlerType { get; set; }
    }
}
