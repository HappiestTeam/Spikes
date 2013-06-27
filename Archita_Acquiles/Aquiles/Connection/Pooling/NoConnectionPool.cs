using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aquiles.Model.Internal;
using Aquiles.Logging;
using Aquiles.Connection.Factory;
using Aquiles.Configuration;

namespace Aquiles.Connection.Pooling
{
    internal class NoConnectionPool : IConnectionPool
    {
        private LoggerHelper logger;

        public NoConnectionPool()
        {
            this.logger = LoggerHelper.CreateLogger(typeof(NoConnectionPool));
        }

        #region IConnectionPool Members

        public string Name
        {
            get;
            set;
        }

        public Aquiles.Connection.Endpoint.IEndpointManager EndpointManager
        {
            set;
            private get;
        }

        public void Initialize()
        {
            //do nothing!!!
        }

        public SpecialConnectionParameterCollection SpecialConnectionParameters
        {
            set;
            private get;
        }

        public Aquiles.Model.Internal.CassandraClient Borrow()
        {
            CassandraEndpoint endpoint = this.EndpointManager.Retrieve();
            if (endpoint != null)
            {
                CassandraClient client = this.ConnectionFactory.Create(endpoint);
                client.OpenTransport();
                return client;
            }
            else
            {
                this.logger.Warn("No client could be borrowed, because no endpoint could be retrieved.");
                return null;
            }
        }

        public void Release(CassandraClient cassandraClient)
        {
            cassandraClient.CloseTransport();
        }

        public void Invalidate(CassandraClient cassandraClient)
        {
            cassandraClient.CloseTransport();
        }


        public IConnectionFactory ConnectionFactory
        {
            set;
            private get;
        }

        #endregion
    }
}
