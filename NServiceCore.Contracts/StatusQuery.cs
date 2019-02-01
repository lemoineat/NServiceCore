using NServiceCore.Middleware.Routing;
using System;
using System.Collections.Generic;
using System.Text;

namespace NServiceCore.Contracts
{
    [Route("/", "GET")]
    [Route("/status", "GET")]
    [Route("/api/v1/status", "GET")]
    public class StatusQuery : IReturn<string>
    {

    }
}
