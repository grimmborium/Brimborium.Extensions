namespace Brimborium.Extensions.RequestPipe {
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Extensions.DependencyInjection;

    using Xunit;

    public class RequestExecutionServiceTests {

        [Fact]
        public void RequestExecutionService_1_ArgumentNullException_Test() {
            {
                Assert.Throws<ArgumentNullException>(() => {
                    var sut = new RequestExecutionService(new RequestPipeOptions(), (IServiceProvider)null);
                    return sut;
                });
            }
            {
                Assert.Throws<ArgumentNullException>(() => {
                    var sut = new RequestExecutionService(new RequestPipeOptions(), (IRequestHandlerFactory)null);
                    return sut;
                });
            }
            {
                    var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
                    using (var serviceProviderRoot = services.BuildServiceProvider()) {
                        var sut = new RequestExecutionService((RequestPipeOptions)null!, serviceProviderRoot);
                        Assert.NotNull( sut);
                    }
            }
            {
                var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
                services.AddRequestPipe();
                using (var serviceProviderRoot = services.BuildServiceProvider()) {
                    
                    var sut = serviceProviderRoot.GetRequiredService<IRequestExecutionService>();
                    Assert.NotNull(sut);

                    Assert.ThrowsAsync<ArgumentNullException>(() => {
                        return sut.ExecuteAsync<object>(null, CancellationToken.None, null);
                    });
                }
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
                            return sut.ExecuteAsync<object>(null, CancellationToken.None, null);
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


        public class RequestGna : IRequest<ResponseGna> {
            public int A { get; set; }
            public int B { get; set; }
        }

        public class ResponseGna {
            public int Sum { get; set; }
        }

        public class RequestHandlerGna : IRequestHandler<RequestGna, ResponseGna> {
            public Task<ResponseGna> ExecuteAsync(
                RequestGna request,
                CancellationToken cancellationToken,
                IRequestHandlerExecutionContext executionContext) {
                var result = request.A + request.B;
                return Task.FromResult<ResponseGna>(new ResponseGna() { Sum = result });
            }

        }

        [Fact]
        public async Task RequestExecutionService_3_Test() {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            services.AddRequestPipe(
                null,
                (builder) => {
                    builder.AddRequestHandler<RequestHandlerGna>();
                });
            using (var serviceProviderRoot = services.BuildServiceProvider()) {
                using (var scope = serviceProviderRoot.CreateScope()) {
                    var serviceProviderScope = scope.ServiceProvider;
                    var sut = serviceProviderScope.GetRequiredService<IRequestExecutionService>();
                    Assert.NotNull(sut);
                    var act = await sut.ExecuteAsync(new RequestGna() { A = 40, B = 2 }, CancellationToken.None, null);
                    Assert.Equal(42, act.Sum);
                }
            }
        }

        public class RequestHandlerFour : RequestHandler<RequestGna, ResponseGna> {
            protected override Task<ResponseGna> HandleAsync(
                RequestGna request,
                CancellationToken cancellationToken,
                IRequestHandlerExecutionContext executionContext) {
                var result = request.A + request.B;
                return Task.FromResult<ResponseGna>(new ResponseGna() { Sum = result });
            }
        }

        [Fact]
        public void RequestExecutionService_4_Test() {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            services.AddRequestPipe((configure) => {
            }, (builder) => {
                builder.AddRequestHandler<RequestHandlerFour>();
            });
            using (var serviceProviderRoot = services.BuildServiceProvider()) {
                using (var scope = serviceProviderRoot.CreateScope()) {
                    var serviceProviderScope = scope.ServiceProvider;
                    var sut = serviceProviderScope.GetRequiredService<IRequestExecutionService>();
                    Assert.NotNull(sut);
                }
            }

        }

    }
}
