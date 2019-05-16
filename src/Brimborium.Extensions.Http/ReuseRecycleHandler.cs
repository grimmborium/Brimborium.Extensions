﻿namespace Brimborium.Extensions.Http {
    using Brimborium.Extensions.Http.Logging;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    using System;
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The 2cd outmost handler in the stack.
    /// This is the first handler that will be reused.
    /// </summary>
    public class ReuseRecycleHandler : DelegatingHandler, IDisposable {
        private readonly ILogger _Logger;
        private IServiceScope _Scope;

        /// <summary>ctor</summary>
        /// <param name="innerHandler">the next inner handler.</param>
        /// <param name="configuration">the configuration to use</param>
        /// <param name="scope">the scope.</param>
        /// <param name="logger">the logger - can be null.</param>
        public ReuseRecycleHandler(
            HttpMessageHandler innerHandler,
            IServiceScope scope,
            ILogger logger
            ) : base(innerHandler) {
            this._Scope = scope;
            this._Logger = logger;
        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }

            var stopwatch = ValueStopwatch.StartNew();
            using (Log.BeginRequestPipelineScope(this._Logger, request)) {
                Log.RequestPipelineStart(this._Logger, request);
                var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
                Log.RequestPipelineEnd(this._Logger, response, stopwatch.GetElapsedTime());
                return response;
            }
        }

        protected override void Dispose(bool disposing) {
            using (var scope = this._Scope) {
                base.Dispose(disposing);
                this._Scope = null;
            }
        }

        private static class Log {
            public static class EventIds {
                public static readonly EventId PipelineStart = new EventId(100, "RequestPipelineStart");
                public static readonly EventId PipelineEnd = new EventId(101, "RequestPipelineEnd");

                public static readonly EventId RequestHeader = new EventId(102, "RequestPipelineRequestHeader");
                public static readonly EventId ResponseHeader = new EventId(103, "RequestPipelineResponseHeader");
            }

            private static readonly Func<ILogger, HttpMethod, Uri, IDisposable> _beginRequestPipelineScope = LoggerMessage.DefineScope<HttpMethod, Uri>("HTTP {HttpMethod} {Uri}");

            private static readonly Action<ILogger, HttpMethod, Uri, Exception> _requestPipelineStart = LoggerMessage.Define<HttpMethod, Uri>(
                LogLevel.Information,
                EventIds.PipelineStart,
                "Start processing HTTP request {HttpMethod} {Uri}");

            private static readonly Action<ILogger, double, HttpStatusCode, Exception> _requestPipelineEnd = LoggerMessage.Define<double, HttpStatusCode>(
                LogLevel.Information,
                EventIds.PipelineEnd,
                "End processing HTTP request after {ElapsedMilliseconds}ms - {StatusCode}");

            public static IDisposable BeginRequestPipelineScope(ILogger logger, HttpRequestMessage request) {
                if ((object)logger != null) {
                    return _beginRequestPipelineScope(logger, request.Method, request.RequestUri);
                } else {
                    return null;
                }
            }

            public static void RequestPipelineStart(ILogger logger, HttpRequestMessage request) {
                if ((object)logger != null) {
                    _requestPipelineStart(logger, request.Method, request.RequestUri, null);

                    if (logger.IsEnabled(LogLevel.Trace)) {
                        logger.Log(
                            LogLevel.Trace,
                            EventIds.RequestHeader,
                            new HttpHeadersLogValue(HttpHeadersLogValue.Kind.Request, request.Headers, request.Content?.Headers),
                            null,
                            (state, ex) => state.ToString());
                    }
                }
            }

            public static void RequestPipelineEnd(ILogger logger, HttpResponseMessage response, TimeSpan duration) {
                if ((object)logger != null) {
                    _requestPipelineEnd(logger, duration.TotalMilliseconds, response.StatusCode, null);

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
}
