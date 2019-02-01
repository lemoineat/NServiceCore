using System;
using System.Collections.Generic;
using System.Text;

namespace NServiceCore.Middleware.Routing
{
    //TODO define attribute scope
    [AttributeUsage(AttributeTargets.Class, AllowMultiple=true)]
    public class RouteAttribute : Attribute
    {
        /// <summary>
        /// Defines a Path for this contract.
        /// Multiple Routes can be added to the same contract but must be of the same Verb
        /// </summary>
        /// <param name="endpoint"></param>
        /// <param name="method"></param>
        public RouteAttribute(string endpoint, string method)
        {
#if DEBUG
            if (string.IsNullOrWhiteSpace(endpoint))
                throw new ArgumentException("message", nameof(endpoint));
            if (string.IsNullOrWhiteSpace(endpoint))
                throw new ArgumentException("message", nameof(method));
            if (endpoint[0] != '/')
                throw new ArgumentException("Endpoint needs to begin with /");
            //todo Method is a known verb
#endif
            Endpoint = endpoint;
            Method = method;
        }

        public string Endpoint { get; set; }

        public string Method { get; set; }
    }
}
