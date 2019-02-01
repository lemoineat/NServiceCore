using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace NServiceCore.Middleware.Routing
{
    public class ContractRouteInitializer
    {
        public RouteMapper Mapper { get; }
        public IEnumerable<Type> ContractsType { get; private set; }

        public ContractRouteInitializer(Assembly contractsAssembly)
        {
            Mapper = new RouteMapper();
            BuildContractStore(contractsAssembly);
        }

        private void BuildContractStore(Assembly contractsAssembly)
        {
            var contracts = FindContracts(contractsAssembly);
            ContractsType = contracts;
            foreach (var contract in contracts)
            {
                var routeAttr = contract.GetCustomAttributes<RouteAttribute>();
                ValidateRoutesOnType(contract, routeAttr);

                foreach (var attr in routeAttr)
                {
                    Mapper.AddRoute(contract, attr.Endpoint, attr.Method);
                }
            }
        }


        private List<Type> FindContracts(Assembly contractsAssembly)
        {
            List<Type> routeContracts = new List<Type>();
            foreach (Type t in contractsAssembly.GetTypes())
            {
                if (t.IsDefined(typeof(RouteAttribute)))
                    routeContracts.Add(t);
            }
            return routeContracts;
        }

        private void ValidateRoutesOnType(Type t, IEnumerable<RouteAttribute> routeAttributes)
        {
            if(routeAttributes.GroupBy(a => a.Method).Count() > 1)
                throw new RouteValidationException($"Type {t.Name} has multiple routes with different Verbs");
        }
    }
}
