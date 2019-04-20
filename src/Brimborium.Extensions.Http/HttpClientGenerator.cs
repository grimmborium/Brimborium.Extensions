namespace Brimborium.Extensions.Http {
    using System;
    using System.Collections.Concurrent;
    using System.Net.Http;
    public class HttpClientGenerator : IHttpClientGenerator {
        private readonly ConcurrentDictionary<HttpClientConfiguration, HttpClientRecycler> _Recyclers;
        public HttpClientGenerator() {
            this._Recyclers = new ConcurrentDictionary<HttpClientConfiguration, HttpClientRecycler>();
        }

        public virtual HttpClient CreateHttpClient(HttpClientConfiguration configuration) {
            throw new System.NotImplementedException();
        }

        public HttpClientCaretaker GetHttpClient(HttpClientConfiguration configuration) {
            HttpClientCaretaker result = null;
            while (true) {
                if (this._Recyclers.TryGetValue(configuration, out var recycler)) {
                    result = recycler.GetHttpClient();
                    break;
                } else {
                    recycler = new HttpClientRecycler(this, configuration);
                    if (!this._Recyclers.TryAdd(configuration, recycler)) {
                        continue;
                    } else {
                        result = recycler.GetHttpClient();
                        break;
                    }
                }
            }
            if (result == null) {
                var client = this.CreateHttpClient(configuration);
                result = new HttpClientCaretaker(client, configuration, this.OnDisposeHttpClient);
            }
            return result;
        }

        private void OnDisposeHttpClient(HttpClientCaretaker caretaker) {            
            var (httpClient, configuration) = caretaker.WrestHttpClient();
            if (httpClient == null) { return; }
            if (configuration == null) { return; }
            if (this._Recyclers.TryGetValue(configuration, out var recycler)) {
                recycler.Recycle(httpClient);
            } else {
                httpClient.Dispose();
            }
             
        }
    }
}
