namespace Microsoft.Extensions.DependencyInjection {
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Brimborium.Extensions.RequestPipe;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public static class RequestPipeExtension {
        public static IServiceCollection AddRequestPipe(this IServiceCollection services, Action<Options> configure = null) {
            if (configure != null) {
                services.Configure(configure);
            }
            services.TryAddScoped<IRequestExecutionService, RequestExecutionService>();

            return services;
        }
    }
}
