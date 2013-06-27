using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aquiles.Configuration;
using Aquiles.Connection.Endpoint;
using Aquiles.Connection.Pooling;
using Aquiles.Connection.Factory;
using Aquiles.Model.Internal;
using Aquiles.Command;
using Aquiles.Exceptions;
using Apache.Cassandra;
using Aquiles.Model;
using System.Globalization;

namespace Aquiles.Impl
{
    internal class DefaultAquilesCluster : IAquilesCluster
    {
        private IEndpointManager endpointManager;
        private IConnectionPool connectionPool;
        private IConnectionFactory connectionFactory;

        public DefaultAquilesCluster(CassandraClusterElement configuration)
        {
            this.KeySpaces = new Dictionary<string, AquilesKeyspace>();
            this.FriendlyName = configuration.FriendlyName;

            this.connectionFactory = ConnectionFactoryHelper.Create(configuration.Connection.FactoryType);
            this.connectionFactory.SpecialConnectionParameters = configuration.Connection.SpecialConnectionParameters;
            this.endpointManager = EndpointManagerHelper.Create(configuration.EndpointManager);
            this.connectionPool = ConnectionPoolHelper.Create(configuration.Connection.PoolType);
            this.connectionPool.Name = String.Format(CultureInfo.InvariantCulture, "{0}_Pool", this.FriendlyName);
            this.endpointManager.ConnectionFactory = this.connectionFactory;
            this.connectionPool.ConnectionFactory = this.connectionFactory;
            this.connectionPool.EndpointManager = this.endpointManager;
            this.connectionPool.SpecialConnectionParameters = configuration.Connection.SpecialConnectionParameters;
        }

        #region IAquilesCluster Members

        public string FriendlyName
        {
            get;
            private set;
        }

        public string ClusterName
        {
            get;
            private set;
        }

        public Dictionary<String,AquilesKeyspace> KeySpaces
        {
            get;
            private set;
        }

        public IConnectionPool ConnectionPool
        {
            get
            {
                return this.connectionPool;
            }
        }

        public void Initialize()
        {
            this.connectionFactory.Initialize();
            // initialize pool (in case it supports warm-up
            this.connectionPool.Initialize();
            this.CheckNodesPointsToSameCluster();
            this.RetrieveKeySpaces();
        }

        #endregion

        private void RetrieveKeySpaces()
        {
            RetrieveKeySpacesCommand retrieveKeySpaceCommand = new RetrieveKeySpacesCommand();
            DescribeKeySpaceCommand describeKeySpaceCommand = new DescribeKeySpaceCommand();
            using (DefaultAquilesConnection connection = new DefaultAquilesConnection(this))
            {
                // i open the conection myself in order not to let the DefaultConnection change the endpoint 
                connection.Open();
                connection.Execute(retrieveKeySpaceCommand);
                HashSet<string>.Enumerator keySpaceIterator = retrieveKeySpaceCommand.KeySpaces.GetEnumerator();
                while (keySpaceIterator.MoveNext()) {
                    describeKeySpaceCommand.KeySpace = keySpaceIterator.Current;
                    connection.Execute(describeKeySpaceCommand);
                    this.AddKeySpaceDescription(this.BuildKeySpace(keySpaceIterator.Current, describeKeySpaceCommand.ColumnFamilies));
                }
                connection.Close();
            }
        }

        private void AddKeySpaceDescription(AquilesKeyspace keyspace)
        {
            this.KeySpaces.Add(keyspace.Name, keyspace);
        }

        private AquilesKeyspace BuildKeySpace(string keySpaceName, Dictionary<string, AquilesColumnFamily> columnFamilies)
        {
            AquilesKeyspace keySpace = new AquilesKeyspace();
            keySpace.Name = keySpaceName;
            keySpace.ColumnFamilies = columnFamilies;

            return keySpace;
        }

        private void CheckNodesPointsToSameCluster()
        {
            CassandraClient client = null;
            HashSet<CassandraEndpoint> usedEndpoints = new HashSet<CassandraEndpoint>();
            FakedConnectionPool fakedConnectionPool = new FakedConnectionPool() 
                { 
                    EndpointManager = this.endpointManager 
                };
            FakedCluster fakedCluster = new FakedCluster() 
                { 
                    FriendlyName = this.FriendlyName,
                    ConnectionPool = fakedConnectionPool
                };
            CassandraEndpoint endpoint = this.endpointManager.Retrieve();
            while (endpoint != null && !usedEndpoints.Contains(endpoint))
            {
                client = this.connectionFactory.Create(endpoint);
                fakedConnectionPool.InjectedClient = client;
                RetrieveClusterNameCommand cmd = new RetrieveClusterNameCommand();
                using (DefaultAquilesConnection connection = new DefaultAquilesConnection(fakedCluster))
                {
                    // i open the conection myself in order not to let the DefaultConnection change the endpoint 
                    connection.Open();
                    connection.Execute(cmd);
                    connection.Close();
                }
                if (String.IsNullOrEmpty(this.ClusterName))
                {
                    this.ClusterName = cmd.ClusterName;
                }
                else if (this.ClusterName.CompareTo(cmd.ClusterName) != 0)
                {
                    throw new AquilesException(String.Format(CultureInfo.CurrentCulture, "Endpoint '{0}' points to '{1}' instead of '{2}'.", endpoint.ToString(), cmd.ClusterName, this.ClusterName));
                }
                usedEndpoints.Add(endpoint);
                endpoint = this.endpointManager.Retrieve();
            }

        }

        internal class FakedCluster : IAquilesCluster
        {
            #region IAquilesCluster Members

            public string FriendlyName
            {
                get;
                set;
            }

            public string ClusterName
            {
                get { return null; }
            }

            public Dictionary<String,AquilesKeyspace> KeySpaces
            {
                get { return null; }
            }

            public IConnectionPool ConnectionPool
            {
                get;
                set;
            }

            public void Initialize()
            {
                //DO NOTHING
            }

            #endregion
        }
        internal class FakedConnectionPool : IConnectionPool
        {
            public CassandraClient InjectedClient
            {
                get;
                set;
            }

            #region IConnectionPool Members
            
            public string Name
            {
                get;
                set;
            }

            public IEndpointManager EndpointManager
            {
                set;
                get;
            }

            public SpecialConnectionParameterCollection SpecialConnectionParameters
            {
                set;
                get;
            }

            public void Initialize()
            {
                //DO NOTHING
            }

            public CassandraClient Borrow()
            {
                this.InjectedClient.OpenTransport();
                return this.InjectedClient;
            }

            public void Release(CassandraClient cassandraClient)
            {
                this.InjectedClient.CloseTransport();
            }

            public void Invalidate(CassandraClient cassandraClient)
            {
                this.InjectedClient.CloseTransport();
                // lets hurry and invalidate this endpoint for future use =)
                if (this.EndpointManager != null)
                {
                    this.EndpointManager.Ban(cassandraClient.Endpoint);
                }
            }

            public IConnectionFactory ConnectionFactory
            {
                set;
                get;
            }

            #endregion
        }
    }
}
