namespace Brimborium.Extensions.Http.Test {
    using System;
    using System.Collections.Generic;
    using System.Text;

    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.DependencyInjection;

    public static class TestHelper {
        public static ServiceProvider CreateServiceProvider(Action<IServiceCollection> addServices) {
            var services = new Microsoft.Extensions.DependencyInjection.ServiceCollection();
            services.AddLogging((loggingBuilder) => {
                loggingBuilder.AddDebug();
            });
            if (addServices != null) {
                addServices(services);
            }
            return services.BuildServiceProvider(true);
        }
    }
}
