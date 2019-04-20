namespace Brimborium.Extensions.Http {
    using System;
    using System.Net.Http;

    public class HttpClientCaretaker : IDisposable {
        private HttpClient _HttpClient;
        private Action<HttpClientCaretaker> _OnDispose;

        public HttpClientCaretaker(
            HttpClient httpClient,
            HttpClientConfiguration configuration,
            Action<HttpClientCaretaker> onDispose
            ) {
            this._HttpClient = httpClient;
            this.Configuration = configuration;
            this._OnDispose = onDispose;
        }

        public bool IsDisposed => (this._HttpClient == null);

        public HttpClient HttpClient {
            get {
                if (this._HttpClient == null) { throw new ObjectDisposedException(nameof(HttpClientCaretaker)); }
                return this._HttpClient;
            }
        }

        public HttpClientConfiguration Configuration { get; }

        public (HttpClient httpClient, HttpClientConfiguration configuration) WrestHttpClient() {
            var httpClient = System.Threading.Interlocked.Exchange(ref this._HttpClient,null);
            this._OnDispose = null;
            return (httpClient, this.Configuration);
        }

        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                // give it back
                System.Threading.Interlocked.Exchange(ref this._OnDispose, null)?.Invoke(this);
                this._HttpClient = null;
            } else {
                // disposing while GC finalizer
                try {
                    using (var httpClient = this._HttpClient) {
                        this._OnDispose = null;
                        this._HttpClient = null;
                    }
                } catch {
                    // no way to log
                }
            }
        }

        ~HttpClientCaretaker() {
            this.Dispose(false);
        }

        public void Dispose() {
            System.GC.SuppressFinalize(this);
            this.Dispose(true);
        }
    }
}
