namespace Brimborium.Extensions.RequestPipe {
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    using Xunit;

    public class PlayTests {
        [Fact]
        public async Task PlayTest001Direct() {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            services.AddScoped<PlayService>();
            using (var serviceProviderRoot = services.BuildServiceProvider()) {
                using (var scope = serviceProviderRoot.CreateScope()) {
                    var scopeServices = scope.ServiceProvider;
                    PlayRequestHandler handler = PlayRequestHandler.Create(scopeServices);
                    var responce = await handler.ExecuteAsync(
                        new PlayRequest() { A = 2, B = 40 },
                        CancellationToken.None,
                        new RequestHandlerExecutionContext());
                    Assert.Equal(42, responce.Sum);
                }
            }
        }
        [Fact]
        public async Task PlayTest002() {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            services.AddScoped<PlayService>();
            services.AddRequestPipe(config => {
                //config.Solver
            }, register => { });
            services.AddTransient<IRequestHandler<PlayRequest, PlayResponse>, PlayRequestHandler>();
            using (var serviceProviderRoot = services.BuildServiceProvider()) {
                using (var scope = serviceProviderRoot.CreateScope()) {
                    var scopeServices = scope.ServiceProvider;
                    PlayRequestHandler handler = PlayRequestHandler.Create(scopeServices);
                    var responce = await handler.ExecuteAsync(
                        new PlayRequest() { A = 2, B = 40 },
                        CancellationToken.None,
                        new RequestHandlerExecutionContext());
                    Assert.Equal(42, responce.Sum);
                }
            }
        }

        public class PlayService {
            public PlayResponse Add(PlayRequest request) {
                var playResponse = new PlayResponse();
                playResponse.Sum = request.A + request.B;
                return playResponse;
            }
        }

        public class PlayResponse {
            public int Sum;
        }

        public class PlayRequest : IRequest<PlayResponse> {
            public int A;
            public int B;
        }

        public class PlayRequestHandler : IRequestHandler<PlayRequest, PlayResponse> {
            public static PlayRequestHandler Create(IServiceProvider service)
                => new PlayRequestHandler(service.GetRequiredService<PlayService>());

            private readonly PlayService _PlayService;

            public PlayRequestHandler(PlayService playService) {
                this._PlayService = playService;
            }

            public Task<Response<PlayResponse>> ExecuteAsync(
                PlayRequest request, 
                CancellationToken cancellationToken, 
                IRequestHandlerExecutionContext executionContext) {
                var responce = this._PlayService.Add(request);
                return Response.FromResultOkTask<PlayResponse>(responce);
            }
        }
    }
}
