using System;
using System.Collections.Generic;
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
            var pathParameters = new List<string>();
            var tokenizedPath = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            var currentParent = root;
            for(int i = 0; i < tokenizedPath.Length; i++)
            {
                var component = tokenizedPath[i];
                string componentName = IsPathParameter(t, component) ? ParameterDelimiter  : component;

                //TODO allow configurable case sensitive endpoints.
                componentName = componentName.ToLower();
                if (!currentParent.Children.ContainsKey(componentName))
                {
                    currentParent.Children[componentName] = new RouteKey();
                }
                currentParent = currentParent.Children[componentName];
            }
            //Set the type
            if(currentParent.ContractByVerb.ContainsKey(verb))
            {
                throw new RouteValidationException($"Type {t.Name} and {currentParent.ContractByVerb[verb].Name} conflict for the same endpoint and verb ({verb}). This API does not currently support branching on types.");
            }
            currentParent.ContractByVerb[verb] = t;
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

        internal Type GetMatchingContractType(string path, string verb)
        {
            var tokenizedPath = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
            verb = verb.ToUpper();
            return MatchingContract(tokenizedPath, verb, root);
        }

        private Type MatchingContract(string[] pathTokens, string verb, RouteKey node, int startIndex = 0)
        {
            var currentParent = root;
            if(startIndex == pathTokens.Length)
            {
                if (!node.ContractByVerb.ContainsKey(verb))
                {
                    return null;
                }
                return node.ContractByVerb[verb];
            }
            var component = pathTokens[startIndex];
            //TODO allow configurable case sensitive mapping;
            component = component.ToLower();
            if (node.Children.ContainsKey(component))
            {
                Type possibleMatch = MatchingContract(pathTokens, verb, node.Children[component], startIndex + 1);
                if(possibleMatch != null)
                {
                    return possibleMatch;
                }
            }
            if (node.Children.ContainsKey(ParameterDelimiter))
            {
                Type possibleMatch = MatchingContract(pathTokens, verb, node.Children[ParameterDelimiter], startIndex + 1);
                if (possibleMatch != null)
                {
                    return possibleMatch;
                }
            }
            return null;
        }

        private class RouteKey
        {
            public RouteKey()
            {
                Children = new Dictionary<string, RouteKey>();
                ContractByVerb = new Dictionary<string, Type>();
            }

            internal Dictionary<string, Type> ContractByVerb;

            internal readonly Dictionary<string, RouteKey> Children;
        }
    }

    
}
