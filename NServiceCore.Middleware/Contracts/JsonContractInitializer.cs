using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using NServiceCore.Middleware.Contracts.DataTypeConverters;
using NServiceCore.Middleware.Routing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace NServiceCore.Middleware.Contracts
{
    public class JsonContractInitializer
    {
        private readonly ConverterRegistrar converters;

        public JsonContractInitializer(ConverterRegistrar converters)
        {
            this.converters = converters;
        }

        public object Instantiate(Type t, IEnumerable<PathParameter> pParams, HttpContext context)
        {
            var serializer = new JsonSerializer();
            object contract;
            using (var jsonTextReader = new JsonTextReader(new StreamReader(context.Request.Body)))
            {
                contract = serializer.Deserialize(jsonTextReader, t);
            }
            //No body was sent.
            if(contract == null)
            {
                contract = Activator.CreateInstance(t);
            }

            contract = MapParameters(contract, t, pParams);

            return contract;
        }

        private object MapParameters(object contract, Type t, IEnumerable<PathParameter> pParams)
        {
            foreach(var pParam in pParams)
            {
                var prop = t.GetProperty(pParam.MappedPropertyName);

                var converter = converters.GetConverter(prop.PropertyType);

                prop.GetSetMethod().Invoke(contract, new[] { converter.Invoke(pParam.ParameterValue) });
            }

            //i dont want to do more work so here is a null so i can test!
            return contract;
        }
    }
}
