using System.Net.Http;

namespace Brimborium.Extensions.Http {
    public interface IHttpClientGenerator {
        HttpClient CreateHttpClient(HttpClientConfiguration configuration);
        HttpClientCaretaker GetHttpClient(HttpClientConfiguration configuration);
    }

}
