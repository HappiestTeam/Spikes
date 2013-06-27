using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aquiles.Exceptions;
using Aquiles.Model.Internal;
using Aquiles.Configuration;
using System.Globalization;

namespace Aquiles.Connection.Endpoint
{
    internal static class EndpointManagerHelper
    {
        private static IEndpointManager Create(EndpointManagerType endpointManagerType)
        {
            IEndpointManager endpointManager = null;
            switch (endpointManagerType)
            {
                case EndpointManagerType.ROUNDROBIN:
                    endpointManager = new RoundRobinEndpointManager();
                    break;
                default:
                    throw new AquilesException(String.Format(CultureInfo.CurrentCulture, "No implementation found for '{0}'", endpointManagerType));
            }

            return endpointManager;
        }

        internal static IEndpointManager Create(EndpointManagerElement endpointManagerElement)
        {
            IEndpointManager manager = Create(endpointManagerElement.Type);
            int defaultTimeout = endpointManagerElement.DefaultTimeout;
            foreach (CassandraEndpointElement endpointInfo in endpointManagerElement.CassandraEndpoints)
            {
                if (endpointInfo.Timeout > 0)
                {
                    manager.AddEndpoint(new CassandraEndpoint(endpointInfo.Address, endpointInfo.Port, endpointInfo.Timeout));
                }
                else
                {
                    manager.AddEndpoint(new CassandraEndpoint(endpointInfo.Address, endpointInfo.Port, defaultTimeout));
                }
            }
            return manager;
        }
    }
}
