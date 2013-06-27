using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aquiles.Connection.Endpoint;
using Aquiles.Model.Internal;
using Aquiles.Connection.Factory;
using Aquiles.Configuration;

namespace Aquiles.Connection.Pooling
{
    internal interface IConnectionPool
    {
        string Name
        {
            get;
            set;
        }

        IEndpointManager EndpointManager
        {
            set;
        }

        IConnectionFactory ConnectionFactory
        {
            set;
        }

        SpecialConnectionParameterCollection SpecialConnectionParameters
        {
            set;
        }

        void Initialize();

        CassandraClient Borrow();

        void Release(CassandraClient cassandraClient);

        void Invalidate(CassandraClient cassandraClient);
    }
}
