using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aquiles.Logging;
using Thrift.Transport;
using Thrift.Protocol;
using Aquiles.Model.Internal;
using Apache.Cassandra;
using Aquiles.Configuration;

namespace Aquiles.Connection.Factory
{
    internal class DefaultTransportConnectionFactory : IConnectionFactory
    {
        private LoggerHelper logger;

        public DefaultTransportConnectionFactory()
        {
            this.logger = LoggerHelper.CreateLogger(this.GetType());
        }

        #region CassandraConnectionFactory Members
        public SpecialConnectionParameterCollection SpecialConnectionParameters
        {
            set;
            private get;
        }

        public void Initialize()
        {
            // DO nothing
        }

        public CassandraClient Create(CassandraEndpoint endpoint)
        {
            TTransport transport = null;
            if (endpoint.Timeout == 0)
            {
                transport = new TSocket(endpoint.Address, endpoint.Port);
            }
            else
            {
                transport = new TSocket(endpoint.Address, endpoint.Port, endpoint.Timeout);
            }
            TProtocol protocol = new TBinaryProtocol(transport);
            Cassandra.Client cassandraClient = new Cassandra.Client(protocol);
            CassandraClient client = new CassandraClient(cassandraClient, endpoint);

            this.logger.Debug(logger.StringFormatInvariantCulture("Created a new connection using: '{0}'", endpoint.ToString()));

            return client;
        }

        #endregion
    }
}
