using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aquiles.Model.Internal;
using Aquiles.Configuration;

namespace Aquiles.Connection.Factory
{
    internal interface IConnectionFactory
    {
        CassandraClient Create(CassandraEndpoint endpoint);

        SpecialConnectionParameterCollection SpecialConnectionParameters
        {
            set;
        }

        void Initialize();
    }
}
