namespace Brimborium.Extensions.Http {
    using System;
    using System.Collections.Generic;

    public class HttpClientConfiguration : IEquatable<HttpClientConfiguration> {
        private Dictionary<string, object> _AdditionalProperties;

        public HttpClientConfiguration() {
            this.Name = string.Empty;
            this.Discriminator = string.Empty;
            this.BaseUrl = string.Empty;
            this.PrimaryConfigure = new List<Action<HttpMessageHandlerBuilder>>();
            this.AdditionalConfigure = new List<Action<HttpMessageHandlerBuilder>>();
        }

        public string Name { get; set; }

        public string Discriminator { get; set; }

        public string BaseUrl { get; set; }

        public bool SuppressHandlerScope { get; set; }

        public object Credentials { get; set; }

        public Dictionary<string, object> AdditionalProperties => this._AdditionalProperties ?? (this._AdditionalProperties = new Dictionary<string, object>());

        public List<Action<HttpMessageHandlerBuilder>> PrimaryConfigure { get; }

        public List<Action<HttpMessageHandlerBuilder>> AdditionalConfigure { get; }

        public HttpClientConfiguration Clone() {
            var result = new HttpClientConfiguration();
            result.Name = this.Name;
            result.Discriminator = this.Discriminator;
            result.BaseUrl = this.BaseUrl;
            result.SuppressHandlerScope = this.SuppressHandlerScope;
            result.Credentials = this.Credentials;
            result.PrimaryConfigure.AddRange(this.PrimaryConfigure);
            result.AdditionalConfigure.AddRange(this.AdditionalConfigure);
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
                && (string.Equals(this.BaseUrl, other.BaseUrl, StringComparison.Ordinal))
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
                    ^ ((this.BaseUrl ?? string.Empty).GetHashCode() << 7)
                    ^ ((this.Discriminator ?? string.Empty).GetHashCode() << 14)
                    ;
            }
        }
    }
}
