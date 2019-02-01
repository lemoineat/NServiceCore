using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Text;

namespace NServiceCore.Middleware
{
    public static class Middleware
    {
        public static IApplicationBuilder InjectRoutingLogic(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomRouting>();
        }
    }
}
