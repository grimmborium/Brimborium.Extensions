namespace Brimborium.Extensions.RequestPipe {
    using System;
    public interface IRequestBase {
    }
    public interface IRequest<TResponse> : IRequestBase {
    }
}
