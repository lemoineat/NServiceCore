using System;
using System.Collections.Generic;
using System.Text;

namespace NServiceCore.Middleware.Routing
{
    public class RouteValidationException : Exception
    {
        public RouteValidationException(string message) : base(message)
        {
        }

        public RouteValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
