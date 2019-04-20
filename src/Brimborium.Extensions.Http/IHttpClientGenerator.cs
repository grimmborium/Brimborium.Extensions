namespace Brimborium.Extensions.Http {
    using System.Net.Http;

    public interface IHttpClientGenerator {
        HttpClient CreateHttpClient(string name);
        HttpClient CreateHttpClient(HttpClientConfiguration configuration);
    }
}
