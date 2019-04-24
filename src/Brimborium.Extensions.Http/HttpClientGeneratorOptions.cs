namespace Brimborium.Extensions.Http {
    using System;
    using System.Collections.Generic;

    /// <summary>Options</summary>
    public class HttpClientGeneratorOptions {
        /// <summary>ctor</summary>
        public HttpClientGeneratorOptions() {
            this.Configurations = new List<HttpClientConfiguration>();
        }

        /// <summary>Each configuration will be added to the <see cref="HttpClientGenerator.Configurations"/> by its name.</summary>
        public List<HttpClientConfiguration> Configurations { get; }

        /// <summary>
        /// Add a configuration
        /// </summary>
        /// <param name="name">the name of the configuration</param>
        /// <param name="configure">action to configure</param>
        public void AddConfiguration(string name, Action<HttpClientConfiguration> configure) {
            var configuration = new HttpClientConfiguration() { Name = name};
            if (configure != null) {
                configure(configuration);
            }
            this.Configurations.Add(configuration);
        }
    }
}
