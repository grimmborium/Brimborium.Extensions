namespace Brimborium.Extensions.Access {
    using System.Net;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>think of - how will the client use this??</summary>
    public interface IHttpClientCredentials : ICredentials {
        /// <summary>Sets the credentials asynchronous.</summary>
        /// <param name="client">The http client.</param>
        /// <param name="request">The request.</param>
        /// <param name="url">The URL.</param>
        /// <returns>async</returns>
        Task SetCredentialsAsync(HttpClient client, HttpRequestMessage request, string url);

        /// <summary>Sets the credentials asynchronous.</summary>
        /// <param name="httpClientHandler">the root client handler</param>
        /// <param name="requestMessage">the request.</param>
        /// <param name="cancellationToken">cancel it</param>
        /// <returns>async.</returns>
        Task BeforeSendAsync(HttpClientHandler httpClientHandler, HttpRequestMessage requestMessage, System.Threading.CancellationToken cancellationToken);

        /// <summary>Handle credentials after the request is done. async.</summary>
        /// <param name="httpClientHandler">the root client handler</param>
        /// <param name="requestMessage">the request</param>
        /// <param name="responseMessage">the responce</param>
        /// <param name="cancellationToken">cancel it</param>
        /// <returns>async</returns>
        Task AfterSendAsync(HttpClientHandler httpClientHandler, HttpRequestMessage requestMessage, HttpResponseMessage responseMessage, CancellationToken cancellationToken);
    }
}
