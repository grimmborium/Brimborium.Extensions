using System;
using System.Net.Http;

namespace Brimborium.Extensions.Http {
    public class HttpClientRecycler {
        private HttpClientGenerator _HttpClientGenerator;
        private readonly HttpClientConfiguration _Configuration;

        public HttpClientRecycler(
            HttpClientGenerator httpClientGenerator,
            HttpClientConfiguration configuration
            ) {
            this._HttpClientGenerator = httpClientGenerator;
            this._Configuration = configuration;
        }

        public HttpClientCaretaker GetHttpClient() {
            throw new NotImplementedException();
        }

        public void Recycle(HttpClient httpClient) {
            throw new NotImplementedException();
        }
    }
}
