using System.Threading;
using System.Threading.Tasks;

namespace Brimborium.Extensions.RequestPipe {
    public class RequestHandlerTranslate { 
    }
    public abstract class RequestHandlerTranslate<TResponse>
        : RequestHandlerTranslate
        , IRequestHandlerTranslate<TResponse> {
        public abstract Task<TResponse> Handle(
            IRequest<TResponse> request,
            CancellationToken cancellationToken,
            IRequestHandlerFactory requestHandlerFactory);
    }
}
