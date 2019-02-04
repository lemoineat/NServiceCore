using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using NServiceCore.Middleware.Contracts;
using NServiceCore.Middleware.Handlers;
using NServiceCore.Middleware.Routing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NServiceCore.Middleware
{
    internal class CustomRouting
    {
        private readonly RequestDelegate _next; //We won't use this.

        private readonly RouteMapper routingMapper;
        private readonly HandlerFactory handlerFactory;
        private readonly JsonContractInitializer contractInitializer;

        //public delegate object RunHandlerDelegate()

        public CustomRouting(RequestDelegate next, ContractRouteInitializer routingMapper, HandlerFactory handlerFactory, JsonContractInitializer contractInitializer)
        {
            _next = next;

            this.routingMapper = routingMapper.Mapper;
            this.handlerFactory = handlerFactory;
            this.contractInitializer = contractInitializer;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            string path = context.Request.Path;
            string verb = context.Request.Method;
            var (contractType, pParams) = routingMapper.GetMatchingContractType(path, verb);

            if(contractType == null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
                return;
            }
            var (handler, mInfo) = handlerFactory.ResolveHandler(contractType);
            if (handler == null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotImplemented;
                return;
            }

            //connemt this out and it will jsut return without deserialize and reserialize.
            var contract = contractInitializer.Instantiate(contractType, pParams, context);
            Task runHandler() => (Task)(mInfo?.Invoke(handler, new[] { contract }));

            var TResult = GetTaskResultType(mInfo);
            if (TResult == null)
            {
                await AwaitNoResponse(runHandler);
                return;
            }
            var resp = await AwaitUnserializedResponse(runHandler, TResult);
            //context.Response.Headers["Content-Type"] = "application/json";
            context.Response.ContentType = "application/json";

            WriteResponseToStream(context, resp);
        }

        private async Task<object> AwaitUnserializedResponse(Func<Task> task, Type expectedResultType)
        {
            var awaitable = task();
            await awaitable;

            object result = (object)((dynamic)awaitable).Result;

            return result;
        }

        private async Task AwaitNoResponse(Func<Task> task)
        {
            var awaitable = task();
            await awaitable;
        }

        private Type GetTaskResultType(MethodInfo method)
        {
            var taskType = method.ReturnType;
            if (!taskType.IsGenericType)
            {
                return null;
            }
            var args = taskType.GetGenericArguments();
            return args[0];
        }


        private void WriteResponseToStream(HttpContext context, object responseObject)
        {
            using (StreamWriter writer = new StreamWriter(context.Response.Body))
            using (JsonTextWriter jsonWriter = new JsonTextWriter(writer))
            {
                JsonSerializer ser = new JsonSerializer();
                ser.Serialize(jsonWriter, responseObject);
                jsonWriter.Flush();
            }
        }
    }
}
