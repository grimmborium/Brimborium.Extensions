namespace Brimborium.Extensions.Http {
    using System.Collections.Generic;

    /// <summary>Options</summary>
    public class HttpClientGeneratorOptions {
        /// <summary>ctor</summary>
        public HttpClientGeneratorOptions() {
            this.Configurations = new List<HttpClientConfiguration>();
        }

        /// <summary>Each configuration will be added to the <see cref="HttpClientGenerator.Configurations"/> by its name.</summary>
        public List<HttpClientConfiguration> Configurations { get; }
    }
}
