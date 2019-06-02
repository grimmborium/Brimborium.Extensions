namespace Brimborium.Extensions.Access {
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;

    public interface IHttpClientCredentialsFactory {
        /// <summary>
        /// Gets the authentication mode.
        /// </summary>
        /// <returns>a text which defines the auth mode .e.g Basic, Windows, ....</returns>
        string GetAuthenticationMode();

        /// <summary>
        /// Configures the HTTP client.
        /// </summary>
        /// <param name="currentClient">The current(new) client.</param>
        /// <param name="credentials">The credentials.</param>
        void ConfigureHttpClient(HttpClient currentClient, System.Net.ICredentials credentials);

        /// <summary>
        /// Configures the primary handler.
        /// </summary>
        /// <param name="httpClientHandler">The HTTP client handler.</param>
        /// <param name="credentials">The credentials.</param>
        void ConfigurePrimaryHandler(HttpClientHandler httpClientHandler, ICredentials credentials);

        /// <summary>
        /// Gets the HTTP client credentials.
        /// </summary>
        /// <param name="httpClientCredentialsSource">The HTTP client credentials source.</param>
        /// <returns>the typed credentials.</returns>
        Task<IHttpClientCredentials> GetHttpClientCredentialsAsync(IHttpClientCredentialsSource httpClientCredentialsSource);
    }
}
