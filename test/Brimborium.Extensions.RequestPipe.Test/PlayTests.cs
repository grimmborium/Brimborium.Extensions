namespace Brimborium.Extensions.RequestPipe {
    using System;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;
    using Xunit;

    public class PlayTests {
        [Fact]
        public async Task PlayTest() {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            services.AddScoped<PlayService>();
            using (var serviceProviderRoot = services.BuildServiceProvider()) {
                using (var scope = serviceProviderRoot.CreateScope()) {
                    var scopeServices = scope.ServiceProvider;
                    PlayRequestHandler handler = PlayRequestHandler.Create(scopeServices);
                    var responce = await handler.ExecuteAsync(new PlayRequest() { A=2,B=40});
                    Assert.Equal(42, responce.Sum);
                }
            }
        }

        public class PlayService {
            public PlayResponce Add(PlayRequest request) {
                var playResponce = new PlayResponce();
                playResponce.Sum = request.A + request.B;
                return playResponce;
            }
        }

        public struct PlayResponce  {
            public int Sum;
        }
        
        public struct PlayRequest : IRequest<PlayResponce> {
            public int A;
            public int B;
        }

        public struct PlayRequestHandler : IRequestHandler<PlayRequest, PlayResponce> {
            public static PlayRequestHandler Create(IServiceProvider service)
                => new PlayRequestHandler(service.GetRequiredService<PlayService>());

            private readonly PlayService _PlayService;

            public PlayRequestHandler(PlayService playService) {
                this._PlayService = playService;
            }

            public Task<PlayResponce> ExecuteAsync(PlayRequest request) {
                var responce = this._PlayService.Add(request);
                return Task.FromResult<PlayResponce>(responce);
            }
        }
    }
}
