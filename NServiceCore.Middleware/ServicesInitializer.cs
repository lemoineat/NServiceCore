using Microsoft.Extensions.DependencyInjection;
using NServiceCore.Middleware.Routing;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NServiceCore.Middleware
{
    public static class NServiceCoreMiddlewareServices
    {
        public static void ConfigureServices(IServiceCollection services, Assembly contractAssembly)
        {
            services.AddSingleton(typeof(ContractRouteInitializer), (IServiceProvider sp) => { return new ContractRouteInitializer(contractAssembly); });
        }
    }
}
