using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NServiceCore.Middleware.Handlers
{
    public class HandlerFactory
    {
        private readonly Dictionary<Type, (Type, MethodInfo)> ContractToHandler;
        private readonly IServiceProvider ioc;

        public HandlerFactory(HandlerMapperInitializer initializer, IServiceProvider services)
        {
            ContractToHandler = initializer.ContractToHandler;
            this.ioc = services;
        }

        public (IHandler, MethodInfo) ResolveHandler(Type contract)
        {
            Type handlerType;
            MethodInfo handlingMethod;
            if (!ContractToHandler.ContainsKey(contract))
            {
                return (null, null);
            }
            (handlerType, handlingMethod) = ContractToHandler[contract];
            return (ioc.GetService(handlerType) as IHandler, handlingMethod);
        }
    }
}
