using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aquiles.Core.Cluster;
using Aquiles.Core.Configuration;
using Aquiles.Core.Connection.Factory;
using Aquiles.Cassandra10.Connection.Factory;

namespace Aquiles.Configuration
{
    public sealed class AquilesClusterBuilder : AbstractAquilesClusterBuilder
    {
        /// <summary>
        /// Type of connection used to connect to Cassandra Cluster
        /// </summary>
        public enum ConnectionFactoryType
        {
            /// <summary>
            /// Default connection. It use binary protocol over socket transport
            /// </summary>
            DEFAULT = 0,
            /// <summary>
            /// Buffered connection. It use binary protocol over buffered transport (This is faster than Default)
            /// </summary>
            BUFFERED = 1,
            /// <summary>
            /// Framed connection. It use binary protocol over framed transport (if you are connecting to a nonblocking server (like Java's TNonblockingServer and THsHaServer) 
            /// </summary>
            FRAMED = 2
        }

        private const string CONNECTIONFACTORY_TRANSPORT_BUFFER_SIZE_OPTION = "transportBufferSize";

        protected override IConnectionFactory buildClientFactory(CassandraClusterElement clusterConfig)
        {
            IConnectionFactory connectionFactory = null;
            ConnectionFactoryType connectionFactoryType;
            connectionFactoryType = (ConnectionFactoryType)Enum.Parse(typeof(ConnectionFactoryType), clusterConfig.Connection.FactoryType, true);
            switch (connectionFactoryType)
            {
                case ConnectionFactoryType.BUFFERED:
                    connectionFactory = this.buildBufferedConnectionFactory(clusterConfig);
                    break;
                case ConnectionFactoryType.FRAMED:
                    connectionFactory = this.buildFramedConnectionFactory(clusterConfig);
                    break;
                case ConnectionFactoryType.DEFAULT:
                    connectionFactory = this.buildDefaultConnectionFactory(clusterConfig);
                    break;
                default:
                    throw new NotImplementedException(String.Format("ConnectionFactoryType '{0}' not implemented.", connectionFactoryType));
            }
            return connectionFactory;
        }

        private IConnectionFactory buildDefaultConnectionFactory(CassandraClusterElement clusterConfig)
        {
            DefaultTransportConnectionFactory connectionFactory = new DefaultTransportConnectionFactory();
            return connectionFactory;
        }

        private IConnectionFactory buildFramedConnectionFactory(CassandraClusterElement clusterConfig)
        {
            FramedTransportConnectionFactory connectionFactory = new FramedTransportConnectionFactory();
            return connectionFactory;
        }

        private IConnectionFactory buildBufferedConnectionFactory(CassandraClusterElement clusterConfig)
        {
            BufferedTransportConnectionFactory connectionFactory = new BufferedTransportConnectionFactory();
            SpecialConnectionParameterElement specialConfig = this.retrieveSpecialParameter(clusterConfig.Connection.SpecialConnectionParameters, CONNECTIONFACTORY_TRANSPORT_BUFFER_SIZE_OPTION);
            int intTempValue = 0;
            if (specialConfig != null && Int32.TryParse(specialConfig.Value, out intTempValue))
            {
                connectionFactory.BufferSize = intTempValue;
            }

            return connectionFactory;
        }
    }
}
    