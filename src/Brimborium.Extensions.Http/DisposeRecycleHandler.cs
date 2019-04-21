namespace Brimborium.Extensions.Http {
    using System.Net.Http;

    public class DisposeRecycleHandler : DelegatingHandler {
        private readonly HttpClientRecycler _HttpClientRecycler;

        public DisposeRecycleHandler(
            HttpMessageHandler innerHandler,
            HttpClientRecycler httpClientRecycler
            ) : base(innerHandler) {
            this._HttpClientRecycler = httpClientRecycler;
        }

        protected override void Dispose(bool disposing) {
            this._HttpClientRecycler?.OnDispose(this);
            base.Dispose(disposing);
        }
    }
}
