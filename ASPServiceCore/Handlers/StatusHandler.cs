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
        public async Task<string> Get(StatusQuery query)
        {
            //await Task.Delay(1000);
            return "Hello World";
        }

        public async Task<ExampleResponse> Post(ParametersInPathTest cmd)
        {
            //await Task.Delay(1000);

            return new ExampleResponse
            {
                StringField = cmd.ParameterOne,
                IntField = cmd.NCharacters,
                ChildObject = new ExampleSubObject
                {
                    BoolField = false,
                    DTOField = DateTimeOffset.UtcNow
                }
            };
        }

        public async Task<ExampleResponse> Get(ParametersInPathTest cmd)
        {
            //await Task.Delay(1000);

            return new ExampleResponse
            {
                StringField = cmd.ParameterOne,
                IntField = cmd.NCharacters,
                ChildObject = new ExampleSubObject
                {
                    BoolField = false,
                    DTOField = DateTimeOffset.UtcNow
                }
            };
        }
    }
}
