using NServiceCore.Middleware.Routing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace NServiceCore.Middleware.Handlers
{
    public class HandlerMapperInitializer
    {
        public Dictionary<Type, (Type, MethodInfo)> ContractToHandler { get; }

        public HandlerMapperInitializer(ContractRouteInitializer routes)
        {
            var allContracts = routes.ContractsType;
            var assembly = Assembly.GetEntryAssembly();
            var allHandlers = FindHandlers(assembly);
            ContractToHandler = BuilderMapper(allContracts, allHandlers);
        }

        public IEnumerable<Type> FindHandlers(Assembly endpointAssembly)
        {
            //type extension here?
            return endpointAssembly.GetTypes().Where(t => t.GetInterfaces().Contains(typeof(IHandler)));
        }

        public Dictionary<Type, (Type, MethodInfo)> BuilderMapper(IEnumerable<Type> contracts, IEnumerable<Type> handlers)
        {
            var unmatchedContracts = new HashSet<Type>(contracts);
            Dictionary<Type, (Type, MethodInfo)> handlerMap = new Dictionary<Type, (Type, MethodInfo)>();
            foreach(var handler in handlers)
            {
                var methods = handler.GetMethods(BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
                foreach(var m in methods)
                {
                    var mParams = m.GetParameters();
                    var returnType = m.ReturnType;
                    if (!IsHttpVerb(m.Name) || mParams.Count() != 1 || !IsTaskType(returnType))
                    {
                        continue;
                    }
                    Type matchedContract = null;
                    foreach (var c in unmatchedContracts)
                    {
                        var route = c.GetCustomAttributes<RouteAttribute>().First();
                        if(m.Name.Equals(route.Method, StringComparison.CurrentCultureIgnoreCase))
                        {
                            if (mParams[0].ParameterType == c)
                            {
                                matchedContract = c;
                                break;
                            }
                        }
                    }
                    if (matchedContract != null)
                    {
                        handlerMap[matchedContract] = (handler, m);
                        unmatchedContracts.Remove(matchedContract);
                    }
                }
            }
            //TODO Log a warning for unmatched routes.
            return handlerMap;
        }

        private bool IsHttpVerb(string name)
        {
            name = name.ToUpper();
            return new[] { "GET", "HEAD", "POST", "PUT", "DELETE", "CONNECT", "OPTIONS", "TRACE", "PATCH" }.Contains(name); //Move to cosntants & interfaces
        }

        private bool IsTaskType(Type T)
        {
            if (T.IsGenericType)
                return T.GetGenericTypeDefinition() == typeof(Task<>);
            return T == typeof(Task);
        }
    }
}
