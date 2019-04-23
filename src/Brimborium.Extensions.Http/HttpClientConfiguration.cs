namespace Brimborium.Extensions.Http {
    using System;
    using System.Collections.Generic;
    using System.Net.Http;

    /// <summary>
    /// The Configuration to build a new HttpClient.
    /// (Noramlly) If the Configuration is registered to the generator the configuration should be treated as immutable.
    /// You can modify it, but be aware that clients can exists.
    /// </summary>
    public class HttpClientConfiguration : IEquatable<HttpClientConfiguration> {
        private Dictionary<string, object> _AdditionalProperties;

        /// <summary>ctor</summary>
        public HttpClientConfiguration() {
            this.Name = string.Empty;
            this.Discriminator = string.Empty;
            this.BaseAddress = string.Empty;
            this.PrimaryHandlerConfigurations = new List<Action<HttpMessageHandlerBuilder>>();
            this.AdditionalHandlerConfigurations = new List<Action<HttpMessageHandlerBuilder>>();
            this.HttpClientConfigurations = new List<Action<HttpClient, HttpClientConfiguration>>();
        }

        /// <summary>The Name is a part of the key to reuse the HttpClient based on this configuration.</summary>
        public string Name { get; set; }

        /// <summary>The Discriminator is a part of the key to reuse the HttpClient based on this configuration.</summary>
        public string Discriminator { get; set; }

        /// <summary>
        /// The BaseAddress is the source of the <see cref="System.Net.Http.HttpClient.BaseAddress"/>.
        /// The BaseAddress is a part of the key to reuse the HttpClient based on this configuration.
        /// </summary>
        public string BaseAddress { get; set; }

        /// <summary>
        /// Foreach instance of a HttpClient a Scope is created. Set this to true to suppress this.
        /// </summary>
        public bool SuppressHandlerScope { get; set; }

        /// <summary>The Credentials can be used by a Handler to do the authentication.</summary>
        public object Credentials { get; set; }

        /// <summary>The AdditionalProperties can be used by a Handler.</summary>
        public Dictionary<string, object> AdditionalProperties => this._AdditionalProperties ?? (this._AdditionalProperties = new Dictionary<string, object>());

        /// <summary>This list of action should create or modify the Primay HttpMessageHandler.</summary>
        public List<Action<HttpMessageHandlerBuilder>> PrimaryHandlerConfigurations { get; }

        /// <summary>This list of action should create additiona HttpMessageHandlers.</summary>
        public List<Action<HttpMessageHandlerBuilder>> AdditionalHandlerConfigurations { get; }

        /// <summary>This list of action should configure the HttpClient.</summary>
        public List<Action<HttpClient, HttpClientConfiguration>> HttpClientConfigurations { get; }

        /// <summary>.Clone this.</summary>
        /// <returns>a clone of this.</returns>
        public HttpClientConfiguration Clone() {
            var result = new HttpClientConfiguration {
                Name = this.Name,
                Discriminator = this.Discriminator,
                BaseAddress = this.BaseAddress,
                SuppressHandlerScope = this.SuppressHandlerScope,
                Credentials = this.Credentials
            };
            result.PrimaryHandlerConfigurations.AddRange(this.PrimaryHandlerConfigurations);
            result.AdditionalHandlerConfigurations.AddRange(this.AdditionalHandlerConfigurations);
            result.HttpClientConfigurations.AddRange(this.HttpClientConfigurations);
            if (this._AdditionalProperties != null) {
                foreach (var kv in this._AdditionalProperties) {
                    result.AdditionalProperties.Add(kv.Key, kv.Value);
                }
            }
            return result;
        }

        public bool Equals(HttpClientConfiguration other) {
            if (ReferenceEquals(null, other)) { return false; }
            if (ReferenceEquals(this, other)) { return true; }
            return
                (string.Equals(this.Name, other.Name, StringComparison.Ordinal))
                && (string.Equals(this.BaseAddress, other.BaseAddress, StringComparison.Ordinal))
                && (string.Equals(this.Discriminator, other.Discriminator, StringComparison.Ordinal))
                ;
        }

        public override bool Equals(object obj) {
            if (obj is HttpClientConfiguration other) { return this.Equals(other); }
            return false;
        }

        public override int GetHashCode() {
            unchecked {
                return
                    ((this.Name ?? string.Empty).GetHashCode())
                    ^ ((this.BaseAddress ?? string.Empty).GetHashCode() << 7)
                    ^ ((this.Discriminator ?? string.Empty).GetHashCode() << 14)
                    ;
            }
        }
    }
}
