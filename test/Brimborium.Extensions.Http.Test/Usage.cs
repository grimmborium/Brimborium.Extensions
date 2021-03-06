using System;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Brimborium.Extensions.Http.Test {
    public class Usage {

        private static void registerServices(IServiceCollection services) {
            services.AddHttpClientGenerator();
        }

        [Fact]
        public void Usage1() {
            using (var services = TestHelper.CreateServiceProvider(registerServices)) {
                var sut = services.GetRequiredService<IHttpClientGenerator>();
                var cfg1 = new HttpClientConfiguration();
                var cfg2 = new HttpClientConfiguration();
                var client1 = sut.CreateHttpClient(cfg1);
                var client2 = sut.CreateHttpClient(cfg2);
                Assert.NotSame(client1, client2);
            }
        }

        [Fact]
        public void Usage2() {
            using (var services = TestHelper.CreateServiceProvider(registerServices)) {
                var sut = services.GetRequiredService<IHttpClientGenerator>();
                var cfg1 = new HttpClientConfiguration() { BaseAddress = "https://www.wikipedia.org/" };
                var cfg2 = new HttpClientConfiguration() { BaseAddress = "https://www.wikipedia.org/" };

                var client1 = sut.CreateHttpClient(cfg1);
                var client2 = sut.CreateHttpClient(cfg2);
                Assert.NotSame(client1, client2);

                cfg2.BaseAddress = "https://localhost";
                var client3 = sut.CreateHttpClient(cfg2);
                Assert.NotSame(client2, client3);

                client1.Dispose();
                client2.Dispose();
                client3.Dispose();
            }
        }
    }
}
