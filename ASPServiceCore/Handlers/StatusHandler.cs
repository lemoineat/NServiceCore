using NServiceCore.Contracts;
using NServiceCore.Middleware.Handlers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASPServiceCore.Handlers
{
    public class StatusHandler : IHandler
    {
        public string Get(StatusQuery query)
        {
            return "Hello World";
        }
    }
}
