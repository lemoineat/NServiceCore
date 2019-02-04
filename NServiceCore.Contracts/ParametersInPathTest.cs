using NServiceCore.Middleware.Routing;
using System;
using System.Collections.Generic;
using System.Text;

namespace NServiceCore.Contracts
{
    [Route("/v1/{ParameterOne}/interact", "POST")]
    [Route("/v1/NoParams", "POST")]
    public class ParametersInPathTest : IReturn<ExampleResponse>
    {
        public string ParameterOne { get; set; }

        public int NCharacters { get; set; }
    }

}
