namespace Brimborium.Extensions.Http {
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using System;
    using System.Collections.Concurrent;
    using System.Net.Http;

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

            this.Services = services;
            this.ScopeFactory = scopeFactory;
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

        public ConcurrentDictionary<string, HttpClientConfiguration> Configurations { get; }

        public IServiceProvider Services { get; }

        public IServiceScopeFactory ScopeFactory { get; }

        public ILoggerFactory LoggerFactory { get; }

        public virtual HttpClient CreateHttpClient(string name) {
            if (this.Configurations.TryGetValue(name, out var configuration)) {
                return this.CreateHttpClient(configuration);
            }
            return null;
        }

        public virtual HttpClient CreateHttpClient(HttpClientConfiguration configuration) {
            while (true) {
                if (this._Recyclers.TryGetValue(configuration, out var recycler)) {
                    return recycler.CreateHttpClient();
                } else {
                    recycler = new HttpClientRecycler(this, configuration);
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
