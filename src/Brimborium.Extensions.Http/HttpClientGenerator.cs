namespace Brimborium.Extensions.Http {
    using System;
    using System.Collections.Concurrent;
    using System.Net.Http;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;

    /// <summary>
    /// A factory for HttpClient.
    /// </summary>
    /// <remarks>This is called generator to avoid confusion / naming clash with HttpClientFactory.</remarks>
    public class HttpClientGenerator : IHttpClientGenerator {
        private readonly ConcurrentDictionary<HttpClientConfiguration, HttpClientRecycler> _Recyclers;

        public HttpClientGenerator(
            IOptions<HttpClientGeneratorOptions> options,
            IServiceProvider services,
            IServiceScopeFactory scopeFactory,
            ILoggerFactory loggerFactory
            ) {
            this._Recyclers = new ConcurrentDictionary<HttpClientConfiguration, HttpClientRecycler>();
            this.Configurations = new ConcurrentDictionary<string, HttpClientConfiguration>(StringComparer.Ordinal);

            this.Services = services ?? throw new ArgumentNullException(nameof(services));
            this.ScopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
            this.LoggerFactory = loggerFactory;

            if (options?.Value != null) {
                var configurations = options.Value.Configurations;
                foreach (var configuration in configurations) {
                    if (string.IsNullOrEmpty(configuration.Name)) {
                        throw new ArgumentException(nameof(options), "configuration.Name is requiered");
                    } else {
                        if (!this.Configurations.TryAdd(configuration.Name, configuration)) {
                            throw new ArgumentException(nameof(options), $"configuration.Name({configuration.Name}) is already used.");
                        }
                    }
                }
            }
        }

        /// <summary>Contains the Configurations</summary>
        public ConcurrentDictionary<string, HttpClientConfiguration> Configurations { get; }

        /// <summary>The Services used by this instance.</summary>
        public IServiceProvider Services { get; }

        /// <summary>The ScopeFactory - used if a new Http​Client​Handler is created.</summary>
        public IServiceScopeFactory ScopeFactory { get; }

        /// <summary>The Factory for Loggers.</summary>
        public ILoggerFactory LoggerFactory { get; }

        /// <summary>Creates a HttpClient based on the Configuration with the given name.</summary>
        /// <param name="name">The name of the Configuration</param>
        /// <returns>A HttpClient or null if no configuration with that name is found.</returns>
        /// <remarks>The HttpClient will be created for each call of this - the underlying HttpClientHandler can be reused/shared.</remarks>
        public virtual HttpClient CreateHttpClient(string name) {
            if (this.Configurations.TryGetValue(name, out var configuration)) {
                return this.CreateHttpClient(configuration);
            }
            return null;
        }

        /// <summary>Creates a HttpClient based on the configuration.</summary>
        /// <param name="configuration"></param>
        /// <returns>A HttpClient based on the configuration</returns>
        /// <remarks>The HttpClient will be created for each call of this - the underlying HttpClientHandler can be reused/shared.</remarks>
        public virtual HttpClient CreateHttpClient(HttpClientConfiguration configuration) {
            while (true) {
                if (this._Recyclers.TryGetValue(configuration, out var recycler)) {
                    return recycler.CreateHttpClient();
                } else {
                    recycler = new HttpClientRecycler(this.Services, this.ScopeFactory, this.LoggerFactory, configuration);
                    if (!this._Recyclers.TryAdd(configuration, recycler)) {
                        continue;
                    } else {
                        return recycler.CreateHttpClient();
                    }
                }
            }
        }
    }
}
