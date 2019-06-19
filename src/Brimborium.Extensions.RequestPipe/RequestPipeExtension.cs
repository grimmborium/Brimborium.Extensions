namespace Microsoft.Extensions.DependencyInjection {
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Brimborium.Extensions.RequestPipe;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.DependencyInjection.Extensions;

    public static class RequestPipeExtension {
        public static IServiceCollection AddRequestPipe(this IServiceCollection services, Action<RequestPipeOptions> configure = null) {
            var optionsBuilder = services.AddOptions<RequestPipeOptions>();
            if (configure != null) {
                optionsBuilder.Configure(configure);
            }
            services.TryAddScoped<IRequestExecutionService, RequestExecutionService>();

            return services;
        }

        //public static IServiceCollection AddRequestPipeFor<TOptions>(this IServiceCollection services, Action<TOptions> configure = null)
        //    where TOptions : RequestPipeOptions, new() {
        //    if (configure != null) {
        //        services.Configure(configure);
        //    }
        //    services.TryAddScoped<IRequestExecutionService<TOptions>, RequestExecutionService<TOptions>>();
        //    return services;
        //}
    }
}
