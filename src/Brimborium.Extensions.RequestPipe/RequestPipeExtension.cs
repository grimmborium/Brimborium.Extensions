using System;
using System.Collections.Generic;
using System.Text;

using Brimborium.Extensions.RequestPipe;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
namespace Microsoft.Extensions.DependencyInjection {

    public static class RequestPipeExtension {
        public static RequestPipeBuilder AddRequestPipe(this IServiceCollection services,
            Action<RequestPipeOptions>? configure = null,
            Action<RequestPipeBuilder>? register = null
            ) {
            if (services is null) { throw new ArgumentNullException(nameof(services)); }
            var optionsBuilder = services.AddOptions<RequestPipeOptions>();
            if (configure != null) {
                optionsBuilder.Configure(configure);
            }

            var requestPipeBuilder = new RequestPipeBuilder(services);
            if (register != null) {
                register(requestPipeBuilder);
            }
            requestPipeBuilder.Build();
            
            return requestPipeBuilder;
        }
    }
}