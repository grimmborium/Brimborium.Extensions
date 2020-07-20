using System;

namespace Brimborium.Extensions.RequestPipe {
    public interface IRequestHandlerOptions {
        Action<Exception>? LogError { get; set; }
    }

    public class RequestHandlerOptions : IRequestHandlerOptions {
        public Action<Exception>? LogError { get; set; }

        public RequestHandlerOptions() {
        }
    }

    public class RequestHandlerFallbackOptions : IRequestHandlerOptions {
        private static IRequestHandlerOptions? _Instance;
        public static IRequestHandlerOptions GetInstance()
            => _Instance ?? (_Instance = new RequestHandlerFallbackOptions());

        public Action<Exception>? LogError { get => null; set { } }

        private RequestHandlerFallbackOptions() {
        }
    }
}
