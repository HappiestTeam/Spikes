using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aquiles.Model.Internal;
using Aquiles.Connection.Factory;

namespace Aquiles.Connection.Endpoint
{
    internal interface IEndpointManager
    {
        void AddEndpoint(CassandraEndpoint cassandraEndpoint);

        List<CassandraEndpoint> Endpoints
        {
            get;
        }

        CassandraEndpoint Retrieve();

        void Ban(CassandraEndpoint endpoint);

        IConnectionFactory ConnectionFactory
        {
            set;
        }
    }
}
