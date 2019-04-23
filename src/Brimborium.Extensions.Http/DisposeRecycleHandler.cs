namespace Brimborium.Extensions.Http {
    using System.Net.Http;

    /// <summary>
    /// The outmost handler - Does nothing while SendAsync.
    /// Part of the Reference-Counting.
    /// </summary>
    public class DisposeRecycleHandler : DelegatingHandler {
        private HttpClientRecycler _HttpClientRecycler;

        /// <summary>ctor</summary>
        /// <param name="innerHandler">the next handler.</param>
        /// <param name="httpClientRecycler">the recycler to to be notified if while Dispose.</param>
        public DisposeRecycleHandler(
            HttpMessageHandler innerHandler,
            HttpClientRecycler httpClientRecycler
            ) : base(innerHandler) {
            this._HttpClientRecycler = httpClientRecycler;
        }

        /// <summary>Calls the recycler - one time.</summary>
        /// <param name="disposing">enables disposing of the inner handler.</param>
        protected override void Dispose(bool disposing) {
            System.Threading.Interlocked.Exchange(ref this._HttpClientRecycler, null)?.OnDispose(this);
            // no not base.Dispose(disposing);
        }

        ~DisposeRecycleHandler() {
            this.Dispose(false);
        }
    }
}
