using Microsoft.Extensions.DependencyInjection;
using NServiceCore.Middleware.Contracts;
using NServiceCore.Middleware.Contracts.DataTypeConverters;
using NServiceCore.Middleware.Handlers;
using NServiceCore.Middleware.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NServiceCore.Middleware
{
    public static class NServiceCoreMiddlewareServices
    {
        public static void ConfigureServices(IServiceCollection services, Assembly contractAssembly)
        {
            services.AddSingleton(typeof(ContractRouteInitializer), (IServiceProvider sp) => { return new ContractRouteInitializer(contractAssembly); });
            services.AddSingleton(typeof(HandlerFactory));
            services.AddSingleton(typeof(HandlerMapperInitializer));
            services.AddSingleton(typeof(JsonContractInitializer));
            services.AddSingleton(typeof(ConverterRegistrar));
                

            //register all handlers
            foreach (var handler in Assembly.GetEntryAssembly().GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IHandler))))
                services.AddTransient(handler);
        }
    }
}
