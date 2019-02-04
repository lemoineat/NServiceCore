using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NServiceCore.Middleware;
using NServiceCore.Contracts;
using NServiceCore.Middleware.Handlers;

namespace ASPServiceCore
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
//            services.AddTransient<IHandler>();
            RegisterHandlers(services);
            NServiceCoreMiddlewareServices.ConfigureServices(services, typeof(StatusQuery).Assembly);
        }

        private void RegisterHandlers(IServiceCollection services)
        {
            this.GetType().Assembly.GetTypes().Where(t => t.IsAssignableFrom(typeof(IHandler)));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.InjectRoutingLogic();
            app.Run(async (context) =>
            {
                await context.Response.WriteAsync("Hello World!");
            });
        }
    }
}
