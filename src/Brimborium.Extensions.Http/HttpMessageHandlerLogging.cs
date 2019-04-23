namespace Brimborium.Extensions.Http {
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    using Brimborium.Extensions.Http.Logging;

    using Microsoft.Extensions.Logging;

    public class HttpMessageHandlerLogging : DelegatingHandler {
        private readonly ILogger _Logger;

        public HttpMessageHandlerLogging(ILogger logger, HttpMessageHandler innerHandler) : base(innerHandler) {
            this._Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }

            var stopwatch = ValueStopwatch.StartNew();

            // Not using a scope here because we always expect this to be at the end of the pipeline, thus there's
            // not really anything to surround.
            Log.RequestStart(this._Logger, request);
            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
            Log.RequestEnd(this._Logger, response, stopwatch.GetElapsedTime());

            return response;
        }

        private static class Log {
            public static class EventIds {
                public static readonly EventId RequestStart = new EventId(100, "RequestStart");
                public static readonly EventId RequestEnd = new EventId(101, "RequestEnd");

                public static readonly EventId RequestHeader = new EventId(102, "RequestHeader");
                public static readonly EventId ResponseHeader = new EventId(103, "ResponseHeader");
            }

            private static readonly Action<ILogger, HttpMethod, Uri, Exception> _requestStart = LoggerMessage.Define<HttpMethod, Uri>(
                LogLevel.Information,
                EventIds.RequestStart,
                "Sending HTTP request {HttpMethod} {Uri}");

            private static readonly Action<ILogger, double, HttpStatusCode, Exception> _requestEnd = LoggerMessage.Define<double, HttpStatusCode>(
                LogLevel.Information,
                EventIds.RequestEnd,
                "Received HTTP response after {ElapsedMilliseconds}ms - {StatusCode}");

            public static void RequestStart(ILogger logger, HttpRequestMessage request) {
                _requestStart(logger, request.Method, request.RequestUri, null);

                if (logger.IsEnabled(LogLevel.Trace)) {
                    logger.Log(
                        LogLevel.Trace,
                        EventIds.RequestHeader,
                        new HttpHeadersLogValue(HttpHeadersLogValue.Kind.Request, request.Headers, request.Content?.Headers),
                        null,
                        (state, ex) => state.ToString());
                }
            }

            public static void RequestEnd(ILogger logger, HttpResponseMessage response, TimeSpan duration) {
                _requestEnd(logger, duration.TotalMilliseconds, response.StatusCode, null);

                if (logger.IsEnabled(LogLevel.Trace)) {
                    logger.Log(
                        LogLevel.Trace,
                        EventIds.ResponseHeader,
                        new HttpHeadersLogValue(HttpHeadersLogValue.Kind.Response, response.Headers, response.Content?.Headers),
                        null,
                        (state, ex) => state.ToString());
                }
            }
        }
    }
}