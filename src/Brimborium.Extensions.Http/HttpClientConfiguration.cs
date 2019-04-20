namespace Brimborium.Extensions.Http {
    using System;

    public class HttpClientConfiguration : IEquatable<HttpClientConfiguration> {
        public HttpClientConfiguration() {
        }

        public string Name { get; set; }

        public string BaseUrl { get; set; }

        //public string AuthenticationMode { get; set; }

        //public object Credentials { get; set; }

        public HttpClientConfiguration Clone() {
            var result = new HttpClientConfiguration();
            result.Name = this.Name;
            result.BaseUrl = this.BaseUrl;
            return result;
        }

        public bool Equals(HttpClientConfiguration other) {
            if (ReferenceEquals(null, other)) { return false; }
            if (ReferenceEquals(this, other)) { return true; }
            return
                (string.Equals(this.Name, other.Name, StringComparison.Ordinal))
                && (string.Equals(this.BaseUrl, other.BaseUrl, StringComparison.Ordinal))
                ;
        }

        public override bool Equals(object obj) {
            if (obj is HttpClientCaretaker other) { return this.Equals(other); }
            return false;
        }

        public override int GetHashCode() {
            unchecked {
                return
                    ((this.Name ?? string.Empty).GetHashCode())
                    ^ ((this.BaseUrl ?? string.Empty).GetHashCode() << 4)
                    ;
            }
        }
    }
}
