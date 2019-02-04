using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NServiceCore.Middleware.Routing
{
    public class RouteMapper
    {
        //private SortedDictionary<string, RouteKey> root;
        private readonly RouteKey root;
        private static readonly string ParameterDelimiter = "^path_parameter_"+Guid.NewGuid().ToString();


        internal RouteMapper()
        {
            root = new RouteKey();
        }

        internal void AddRoute(Type t, string path, string verb)
        {
            verb = verb.ToUpper();
            //var pathParameters = new List<string>();
            var tokenizedPath = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            var currentParent = root;
            List<PathParameter> pathParameters = new List<PathParameter>();
            for(int i = 0; i < tokenizedPath.Length; i++)
            {
                var component = tokenizedPath[i];
                string componentKey = component;
                if (IsPathParameter(t, component))
                {
                    componentKey = ParameterDelimiter;
                    pathParameters.Add(new PathParameter
                    {
                        PathIndex = i,
                        MappedPropertyName = component.Substring(1, component.Length - 2)
                    });
                }

                //TODO allow configurable case sensitive endpoints.
                componentKey = componentKey.ToLower();
                if (!currentParent.Children.ContainsKey(componentKey))
                {
                    currentParent.Children[componentKey] = new RouteKey();
                }
                currentParent = currentParent.Children[componentKey];
            }
            //Set the type
            var conflictingRoute = currentParent.Contracts.FirstOrDefault(c => c.Verb == verb);
            if (conflictingRoute != null)
            {
                throw new RouteValidationException($"Type {t.Name} and {conflictingRoute.ContractType.Name} conflict for the same endpoint and verb ({verb}). This API does not support branching on types.");
            }
            currentParent.Contracts.Add(new RouteLeaf
            {
                ContractType = t,
                Verb = verb,
                OrderedParams = pathParameters
            });
        }

        private bool IsPathParameter(Type t, string component)
        {
            if (component.StartsWith('{') || component.EndsWith('}'))
            {
                if (!component.EndsWith('}') || !component.StartsWith('{')) { throw new RouteValidationException($"Type {t.Name} has an unclosed PathParameter, {component}"); }

                string param = component.Substring(1, component.Length - 2);
                var property = t.GetProperty(param);
                if (property == null)
                {
                    throw new RouteValidationException($"Type {t.Name} is missing a mapable Path Parameter, for {param}");
                }
                return true;
            }
            return false;
        }

        internal (Type,IEnumerable<PathParameter>) GetMatchingContractType(string path, string verb)
        {
            var tokenizedPath = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            verb = verb.ToUpper();
            var bestRoute = MatchingContract(tokenizedPath, verb, root, new List<PathParameter>());
            var (routeDescrip, pathParams) = bestRoute;
            if(routeDescrip == null)
            {
                return (null, null);
            }
            MapPropertyNames(routeDescrip, pathParams);

            return (routeDescrip.ContractType, pathParams);
        }

        private (RouteLeaf route, List<PathParameter> pathParams) MatchingContract(string[] pathTokens, string verb, RouteKey root, List<PathParameter> @params, int startIndex = 0)
        {
            if(startIndex == pathTokens.Length)
            {
                var routeMatch = root.Contracts.FirstOrDefault(leaf => leaf.Verb == verb);
                if (routeMatch == null)
                {
                    return (null, null);
                }
                return (routeMatch, @params);
            }
            var component = pathTokens[startIndex];
            //TODO allow configurable case sensitive mapping;
            var componentLower = component.ToLower();
            if (root.Children.ContainsKey(componentLower))
            {
                var (match, pathParams) = MatchingContract(pathTokens, verb, root.Children[componentLower], @params, startIndex + 1);
                if(match != null)
                {
                    return (match, pathParams);
                }
            }
            if (root.Children.ContainsKey(ParameterDelimiter))
            {
                @params.Add(new PathParameter
                {
                    PathIndex = startIndex,
                    MappedPropertyName = null,
                    ParameterValue = component
                });
                var (match, pathParams) = MatchingContract(pathTokens, verb, root.Children[ParameterDelimiter], @params, startIndex + 1);
                if (match != null)
                {
                    return (match, pathParams);
                }
            }
            return (null, null);
        }


        private void MapPropertyNames(RouteLeaf route, IList<PathParameter> @params)
        {
            for(int i = 0; i < @params.Count(); i++)
            {
                var param = @params[i];
                param.MappedPropertyName = route.OrderedParams[i].MappedPropertyName;
            }
        }

        private class RouteKey
        {
            public RouteKey()
            {
                Contracts = new List<RouteLeaf>();
                Children = new Dictionary<string, RouteKey>();
                WildcardChildren = new Dictionary<string, RouteKey>();
            }

            internal readonly List<RouteLeaf> Contracts;

            internal readonly Dictionary<string, RouteKey> Children;

            internal readonly Dictionary<string, RouteKey> WildcardChildren;
        }

        internal class RouteLeaf
        {
            internal Type ContractType { get; set; }
            
            internal string Verb { get; set; }

            internal IList<PathParameter> OrderedParams { get; set; }
        }
    }

    
}
