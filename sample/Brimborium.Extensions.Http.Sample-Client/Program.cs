namespace Brimborium.Extensions.Http.Sample_Client {
    using System;
    using System.Net.Http;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public class Program {

        public static async Task Main(string[] args) {
            // Start Brimborium.Extensions.Http.Sample-Server
            // Than start Brimborium.Extensions.Http.Sample-Client
            var urlOfSampleServer = "http://localhost:1234";

            using (var serviceProvider = CreateServices().BuildServiceProvider()) {
                var httpClientGenerator = serviceProvider.GetRequiredService<IHttpClientGenerator>();
                await run1(httpClientGenerator);
                await run2(httpClientGenerator);
                await run3(urlOfSampleServer, httpClientGenerator);
            }
        }

        private static IServiceCollection CreateServices() {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            services.AddOptions();
            services.AddLogging((builder) => {
                builder.AddConsole();
            });
            services.AddHttpClientGenerator((configure) => {
                configure.AddConfiguration("github", (httpClientConfiguration) => {
                    httpClientConfiguration.BaseAddress = "https://www.github.com";
                });
            });
            return services;
        }
        private static async Task run1(IHttpClientGenerator httpClientGenerator) {

            System.Console.Out.WriteLine(
                $"Calling 'https://www.github.com' using the predefined named configuration 'github'"
                );
            using (var httpClient = httpClientGenerator.CreateHttpClient("github")) {
                var responce = await httpClient.GetAsync("");
                System.Console.Out.WriteLine($"StatusCode: {responce.StatusCode}");
            }
            // The httpClient is disposed - but not the socket (PrimaryHandler).
            // Within 2 minutes the socket will be reused - if a CreateHttpClient is called.
            // Being 2 minutes unused the socket will be disposed.
        }

        private static async Task run2(IHttpClientGenerator httpClientGenerator) {
            System.Console.Out.WriteLine(
                    $"Calling 'https://www.github.com' using a dynamic configuration"
                    );
            var configuration = new HttpClientConfiguration() {
                Name = "dynamic",
                Discriminator = "anonymous",
                BaseAddress = "https://www.github.com"
            };
            //
            // To reuse the configuration set it to the Configurations.
            // httpClientGenerator.Configurations[configuration.Name] = configuration;
            // after the configuration is shared don't modify it - unless you know it better.
            //
            // You may / You can do this - You *don't* have to.
            //
            // The fields 
            // configuration.BaseAddress
            // configuration.Name
            // configuration.Discriminator
            // are part of the HashCode / Equals.
            //
            // To reuse the socket in a valid way set the values.
            // configuration.Name: the name as e.g "DynamicForMyAPI"
            // configuration.Discriminator: if you have 2 user you may set Discriminator = username
            //
            /*                         
                        The name is part of the logger
                        info: System.Net.Http.HttpClient.github.LogicalHandler[100]
                            Start processing HTTP request GET https://www.github.com/

                        info: System.Net.Http.HttpClient.dynamic.LogicalHandler[100]
                              Start processing HTTP request GET https://www.github.com/
            */
            using (var httpClient = httpClientGenerator.CreateHttpClient(configuration)) {
                var responce = await httpClient.GetAsync("");
                System.Console.Out.WriteLine($"StatusCode: {responce.StatusCode}");
            }
        }

        private static async Task run3(string urlOfSampleServer, IHttpClientGenerator httpClientGenerator) {
            System.Console.Out.WriteLine(
                    $"Calling '{urlOfSampleServer}' using a dynamic configuration"
                    );
            int loop = 0;
            var configuration = new HttpClientConfiguration() {
                Name = "dynamicHttpSampleServer",
                Discriminator = "anonymous",
                BaseAddress = urlOfSampleServer,
            };
            configuration.PrimaryHandlerConfigurations.Add((builder) => {
                System.Console.Out.WriteLine($"This is called only once in the loop {loop} for PrimaryHandlerConfigurations.");

                // enable windows-auth
                builder.EnsurePrimaryHandler().UseDefaultCredentials = true;

                // you can call 
                // builder.AdditionalHandlers.Add
                // you don't need to do it seperatly
            });
            configuration.AdditionalHandlerConfigurations.Add((builder) => {
                System.Console.Out.WriteLine($"This is called only once in the loop {loop} for AdditionalHandlerConfigurations.");

                builder.AdditionalHandlers.Add(
                    new WhateverDelegatingHandler("you")
                    );
            });

            configuration.HttpClientConfigurations.Add((HttpClient, httpClientConfiguration) => {
                System.Console.Out.WriteLine($"This is called for each request - now in the loop {loop} for HttpClientConfigurations.");
            });

            for (loop = 0; loop < 3; loop++) {
                using (var httpClient = httpClientGenerator.CreateHttpClient(configuration)) {
                    var responce = await httpClient.GetAsync("");
                    var content = await responce.Content.ReadAsStringAsync();
                    System.Console.Out.WriteLine($"Responce: {responce.StatusCode} {content}");
                }
            }
        }
    }

    internal class WhateverDelegatingHandler : System.Net.Http.DelegatingHandler {
        private readonly string _Name;

        public WhateverDelegatingHandler(string name) {
            this._Name = name ?? string.Empty;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken) {
            request.Headers.Add("X-Name", this._Name);
            return base.SendAsync(request, cancellationToken);
        }
    }
}
