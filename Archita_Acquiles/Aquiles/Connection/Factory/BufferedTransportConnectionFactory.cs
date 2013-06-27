using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aquiles.Logging;
using Thrift.Transport;
using Thrift.Protocol;
using Apache.Cassandra;
using Aquiles.Model.Internal;
using Aquiles.Configuration;

namespace Aquiles.Connection.Factory
{
    internal class BufferedTransportConnectionFactory : IConnectionFactory
    {
        private const string TRANSPORT_BUFFER_SIZE_OPTION = "transportBufferSize";
        private LoggerHelper logger;
        private int bufferSize;
        private bool isBufferSizeSet;

        public BufferedTransportConnectionFactory()
        {
            this.logger = LoggerHelper.CreateLogger(this.GetType());
            this.isBufferSizeSet = false;
            this.bufferSize = 0;
        }

        #region CassandraConnectionFactory Members
        
        public SpecialConnectionParameterCollection SpecialConnectionParameters
        {
            set;
            private get;
        }

        public CassandraClient Create(CassandraEndpoint endpoint)
        {
            TSocket socket = null;
            TTransport transport = null;
            if (endpoint.Timeout == 0)
            {
                socket = new TSocket(endpoint.Address, endpoint.Port);
            }
            else
            {
                socket = new TSocket(endpoint.Address, endpoint.Port, endpoint.Timeout);
            }

            if (this.isBufferSizeSet)
            {
                transport = new TBufferedTransport(socket, this.bufferSize);
            }
            else
            {
                transport = new TBufferedTransport(socket);
            }

            TProtocol protocol = new TBinaryProtocol(transport);
            Cassandra.Client cassandraClient = new Cassandra.Client(protocol);
            CassandraClient client = new CassandraClient(cassandraClient, endpoint);

            this.logger.Debug(logger.StringFormatInvariantCulture("Created a new connection using: '{0}'", endpoint.ToString()));

            return client;
        }



        public void Initialize()
        {
            if (this.SpecialConnectionParameters != null)
            {
                SpecialConnectionParameterElement bufferSizeParam = this.SpecialConnectionParameters[TRANSPORT_BUFFER_SIZE_OPTION];
                if (bufferSizeParam != null)
                {
                    this.isBufferSizeSet = int.TryParse(bufferSizeParam.Value, out this.bufferSize) && (this.bufferSize > 0);
                }
            }

            if (this.isBufferSizeSet)
            {
                logger.Info(logger.StringFormatInvariantCulture("Connections will be created using '{0}' bytes of buffer size.", this.bufferSize));
            }
            else
            {
                logger.Info("Connections will be created using DEFAULT buffer size.");
            }
        }
        #endregion
    }
}
