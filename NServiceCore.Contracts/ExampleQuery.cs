using NServiceCore.Middleware.Routing;
using System;
using System.Collections.Generic;
using System.Text;

namespace NServiceCore.Contracts
{
    [Route("/example", "GET")]
    public class ExampleQuery : IReturn<ExampleResponse>
    {
    }

    public class ExampleResponse
    {
        public string StringField { get; set; }

        public int IntField { get; set; }

        public ExampleSubObject ChildObject { get; set; } 
    }

    public class ExampleSubObject
    {
        public bool BoolField { get; set; }

        public DateTimeOffset DTOField { get; set;}
    }
}
