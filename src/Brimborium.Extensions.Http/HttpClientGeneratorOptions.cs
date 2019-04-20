namespace Brimborium.Extensions.Http {
    using System.Collections.Generic;

    public class HttpClientGeneratorOptions {
        public HttpClientGeneratorOptions() {
            this.Configurations = new List<HttpClientConfiguration>();
        }

        public List<HttpClientConfiguration> Configurations { get; }
    }
}
