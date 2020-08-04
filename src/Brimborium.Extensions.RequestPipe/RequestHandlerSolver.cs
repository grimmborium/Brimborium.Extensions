using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
using System.Threading;
using System.Threading.Tasks;

using Brimborium.Extensions.Decoration;

namespace Brimborium.Extensions.RequestPipe {

    public class RequestHandlerSolver : IRequestHandlerSolver {
        private static Dictionary<Type, Type> _RequestHandlerTranslates = new Dictionary<Type, Type>();
        private static Dictionary<Type, object> _RequestHandlers = new Dictionary<Type, object>();

        private readonly IRequestHandlerFactory _RequestHandlerFactory;

        public RequestHandlerSolver(
            IRequestHandlerFactory requestHandlerFactory) {
            this._RequestHandlerFactory = requestHandlerFactory ?? throw new ArgumentNullException(nameof(requestHandlerFactory));
        }

        public IRootRequestHandler<TResponse> GetRequestHandler<TResponse>(
            IRequest<TResponse> request,
            IRequestHandlerExecutionContext executionContext) {
            if (request is null) {
                throw new ArgumentNullException(nameof(request));
            }
            var requestType = request.GetType();
            var solverTranslate = this.GetSolverTranslate<TResponse>(requestType);
            var rootRequestHandler = solverTranslate.GetRootRequestHandler(request, executionContext);
            return rootRequestHandler;
        }

        internal RequestHandlerSolverTranslate<TResponse> GetSolverTranslate<TResponse>(
            Type requestType) {
            {
                if (_RequestHandlers.TryGetValue(requestType, out var handler)) {
                    return (RequestHandlerSolverTranslate<TResponse>)handler;
                }
            }
            {
                if (_RequestHandlerTranslates.TryGetValue(requestType, out var typeRequestHandlerTranslate)) {
                    //OK
                } else {
                    typeRequestHandlerTranslate = typeof(RequestHandlerSolverTranslate<,>).MakeGenericType(requestType, typeof(TResponse));
                    while (true) {
                        var oldRequestHandlerTranslates = _RequestHandlerTranslates;
                        var requestHandlerTranslates = new Dictionary<Type, Type>(oldRequestHandlerTranslates);
                        if (ReferenceEquals(
                            oldRequestHandlerTranslates,
                            System.Threading.Interlocked.CompareExchange(ref _RequestHandlerTranslates, requestHandlerTranslates, oldRequestHandlerTranslates))) {
                            System.Threading.Interlocked.MemoryBarrier();
                            break;
                        }
                    }
                }
                //
                var newHandler = (RequestHandlerSolverTranslate<TResponse>)Activator.CreateInstance(typeRequestHandlerTranslate, this);
                while (true) {
                    var oldRequestHandlers = _RequestHandlers;
                    var requestHandlers = new Dictionary<Type, object>(oldRequestHandlers);
                    requestHandlers[requestType] = newHandler;
                    if (ReferenceEquals(oldRequestHandlers,
                        System.Threading.Interlocked.CompareExchange(ref _RequestHandlers, requestHandlers, oldRequestHandlers)
                        )) {
                        return newHandler;
                    }
                    if (_RequestHandlers.TryGetValue(requestType, out var handler)) {
                        return (RequestHandlerSolverTranslate<TResponse>)handler;
                    }
                }
            }
        }

        public IRootRequestHandler<TResponse> GetRootRequestHandler<TRequest, TResponse>()
            where TRequest : IRequest<TResponse> {
            IRequestHandler<TRequest, TResponse> requestHandler;
            IEnumerable<IRequestHandlerChain<TRequest, TResponse>>? requestHandlerChains;

            try {
                requestHandler = this._RequestHandlerFactory.CreateRequestHandler<IRequestHandler<TRequest, TResponse>>();
            } catch (Exception error) {
                throw new InvalidOperationException($"Error constructing handler for request of type {typeof(IRequestHandler<TRequest, TResponse>)}.", error);
            }
            if (requestHandler == null) {
                throw new InvalidOperationException($"Handler was not found for request of type {typeof(IRequestHandler<TRequest, TResponse>)}.");
            }
            try {
                requestHandlerChains = this._RequestHandlerFactory.CreateRequestHandlerChains<IRequestHandlerChain<TRequest, TResponse>>();
            } catch (Exception error) {
                throw new InvalidOperationException($"Error constructing handler for request of type {typeof(IRequestHandlerChain<TRequest, TResponse>)}.", error);
            }

            //var lstRequestHandleCfg = new List<RequestHandlerConfiguration>();


            var result = requestHandler;
            if (requestHandlerChains is object) {
                var lstRequestHandlerChain = requestHandlerChains.ToList();
                lstRequestHandlerChain.Sort(IRequestHandlerChainBaseComparer.GetInstance());

                for (int idx = lstRequestHandlerChain.Count - 1; idx >= 0; idx--) {
                    IRequestHandlerChain<TRequest, TResponse>? chain = lstRequestHandlerChain[idx];
                    if (chain is object) {
                        result = new RequestHandlerChainer<TRequest, TResponse>(chain, result);
                    }
                }
            }
            return new RootRequestHandler<TRequest, TResponse>(result);
        }
    }

    internal class RequestHandlerChainer<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
        where TRequest : IRequest<TResponse> {
        private readonly IRequestHandlerChain<TRequest, TResponse> _RequestHandlerChain;
        private readonly IRequestHandler<TRequest, TResponse> _Next;

        public RequestHandlerChainer(
            IRequestHandlerChain<TRequest, TResponse> requestHandlerChain,
            IRequestHandler<TRequest, TResponse> next) {
            this._RequestHandlerChain = requestHandlerChain ?? throw new ArgumentNullException(nameof(requestHandlerChain));
            this._Next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public Task<Response<TResponse>> ExecuteAsync(TRequest request, System.Threading.CancellationToken cancellationToken, IRequestHandlerExecutionContext executionContext) {
            return this._RequestHandlerChain.ExecuteAsync(request, cancellationToken, executionContext, this._Next);
        }
    }

    internal abstract class RequestHandlerSolverTranslate {
    }

    internal abstract class RequestHandlerSolverTranslate<TResponse>
        : RequestHandlerSolverTranslate {

        public abstract IRootRequestHandler<TResponse> GetRootRequestHandler(
            IRequest<TResponse> request,
            IRequestHandlerExecutionContext executionContext);
    }

    internal class RequestHandlerSolverTranslate<TRequest, TResponse>
        : RequestHandlerSolverTranslate<TResponse>

        where TRequest : IRequest<TResponse> {
        private readonly IRequestHandlerSolver _RequestHandlerSolver;

        public RequestHandlerSolverTranslate(IRequestHandlerSolver requestHandlerSolver) {
            this._RequestHandlerSolver = requestHandlerSolver;
        }

        public override IRootRequestHandler<TResponse> GetRootRequestHandler(
            IRequest<TResponse> request,
            IRequestHandlerExecutionContext executionContext) {
            var result = this._RequestHandlerSolver.GetRootRequestHandler<TRequest, TResponse>();
            return result;
        }
    }

    internal class RootRequestHandler<TRequest, TResponse>
        : IRootRequestHandler<TResponse>
        where TRequest : IRequest<TResponse> {
        private readonly IRequestHandler<TRequest, TResponse> _RequestHandler;

        public RootRequestHandler(IRequestHandler<TRequest, TResponse> requestHandler) {
            this._RequestHandler = requestHandler ?? throw new ArgumentNullException(nameof(requestHandler));
        }
        public Task<Response<object?>> ExecuteObjectAsync(object request, CancellationToken cancellationToken, IRequestHandlerExecutionContext executionContext) {
            return this._RequestHandler.ExecuteAsync((TRequest)request, cancellationToken, executionContext)
                .ContinueWith((t) => {
                    if (t.IsFaulted) {
                        return Response.FromException<object?>(t.Exception.InnerException);
                        //ExceptionDispatchInfo.Capture(t.Exception.InnerException).Throw();
                    } else {
                        return Response.FromResultOk<object?>((object?)t.Result);
                    }
                }, cancellationToken);
        }

        public Task<Response<TResponse>> ExecuteTypedAsync(IRequest<TResponse> request, CancellationToken cancellationToken, IRequestHandlerExecutionContext executionContext) {
            return this._RequestHandler.ExecuteAsync((TRequest)request, cancellationToken, executionContext);
        }
    }

    internal class IRequestHandlerChainBaseComparer
        : IComparer<IRequestHandlerChainBase> {
        private static IRequestHandlerChainBaseComparer? _Instance;
        public static IRequestHandlerChainBaseComparer GetInstance()
            => _Instance ??= new IRequestHandlerChainBaseComparer();

        public int Compare(IRequestHandlerChainBase x, IRequestHandlerChainBase y) {
            return x.GetOrder().CompareTo(y.GetOrder());
        }
    }

}
