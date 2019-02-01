using Microsoft.AspNetCore.Http;
using NServiceCore.Middleware.Handlers;
using NServiceCore.Middleware.Routing;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace NServiceCore.Middleware
{
    internal class CustomRouting
    {
        private readonly RequestDelegate _next; //We won't use this.

        private readonly RouteMapper routingMapper;
        private readonly HandlerFactory handlerFactory;

        public CustomRouting(RequestDelegate next, ContractRouteInitializer routingMapper, HandlerFactory handlerFactory)
        {
            _next = next;

            this.routingMapper = routingMapper.Mapper;
            this.handlerFactory = handlerFactory;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string path = context.Request.Path;
            string verb = context.Request.Method;
            var contractType = routingMapper.GetMatchingContractType(path, verb);

            if(contractType == null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }
            var (handler, mInfo) = handlerFactory.ResolveHandler(contractType);
            if(handler == null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotImplemented;
                return;
            }

            //connemt this out and it will jsut return without deserialize and reserialize.
            var task = mInfo?.Invoke(handler, /*Need To create contract*/);
        }
    }
}
