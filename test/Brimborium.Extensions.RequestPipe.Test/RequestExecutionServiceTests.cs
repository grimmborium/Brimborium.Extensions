namespace Brimborium.Extensions.RequestPipe {
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    public class RequestExecutionServiceTests {

        [Fact]
        public void RequestExecutionService_1_ArgumentNullException_Test() {
            {
                Assert.Throws<ArgumentNullException>(() => {
                    var sut = new RequestExecutionService(null, null);
                    return sut;
                });
            }
            {
                var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
                services.AddRequestPipe();
                using (var serviceProviderRoot = services.BuildServiceProvider()) {
                    using (var scope = serviceProviderRoot.CreateScope()) {
                        var serviceProviderScope = scope.ServiceProvider;
                        var sut = serviceProviderScope.GetRequiredService<IRequestExecutionService>();
                        Assert.NotNull(sut);

                        Assert.ThrowsAsync<ArgumentNullException>(() => {
                            return sut.ExecuteAsync<object>(null);
                        });

                    }
                }

            }
        }

        [Fact]
        public void RequestExecutionService_2_Test() {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            services.AddRequestPipe();
            using (var serviceProviderRoot = services.BuildServiceProvider()) {
                using (var scope = serviceProviderRoot.CreateScope()) {
                    var serviceProviderScope = scope.ServiceProvider;
                    var sut = serviceProviderScope.GetRequiredService<IRequestExecutionService>();
                    Assert.NotNull(sut);
                }
            }
        }


        public class RequestGna : IRequest<ResponceGna> {
            public int A { get; set; }
            public int B { get; set; }
        }

        public class ResponceGna {
            public int Sum { get; set; }
        }

        public class RequestHandlerGna : IRequestHandler<RequestGna, ResponceGna> {
            public Task<ResponceGna> ExecuteAsync(RequestGna request) {
                var result = request.A + request.B;
                return Task.FromResult<ResponceGna>(new ResponceGna() { Sum = result });
            }
        }

        [Fact]
        public async Task RequestExecutionService_3_Test() {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            services.AddRequestPipe((configure) => {
                configure.Add<RequestGna, ResponceGna, RequestHandlerGna>();
            });
            using (var serviceProviderRoot = services.BuildServiceProvider()) {
                using (var scope = serviceProviderRoot.CreateScope()) {
                    var serviceProviderScope = scope.ServiceProvider;
                    var sut = serviceProviderScope.GetRequiredService<IRequestExecutionService>();
                    Assert.NotNull(sut);
                    var act = await sut.ExecuteAsync(new RequestGna() { A = 40, B = 2 });
                    Assert.Equal(42, act.Sum);
                }
            }
        }

    }
}
