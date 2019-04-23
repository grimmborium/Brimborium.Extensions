namespace Brimborium.Extensions.Http {
    using System.Net.Http;

    public interface IHttpClientGenerator {
        /// <summary>Contains the Configurations.</summary>
        System.Collections.Concurrent.ConcurrentDictionary<string, HttpClientConfiguration> Configurations { get; }

        /// <summary>Creates a HttpClient based on the Configuration with the given name.</summary>
        /// <param name="name">The name of the Configuration</param>
        /// <returns>A HttpClient or null if no configuration with that name is found.</returns>
        /// <remarks>The HttpClient will be created for each call of this - the underlying HttpClientHandler can be reused/shared.</remarks>
        HttpClient CreateHttpClient(string name);

        /// <summary>Creates a HttpClient based on the configuration.</summary>
        /// <param name="configuration"></param>
        /// <returns>A HttpClient based on the configuration</returns>
        /// <remarks>The HttpClient will be created for each call of this - the underlying HttpClientHandler can be reused/shared.</remarks>
        HttpClient CreateHttpClient(HttpClientConfiguration configuration);
    }
}
