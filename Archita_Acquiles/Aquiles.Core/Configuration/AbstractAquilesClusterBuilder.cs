using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aquiles.Core.Cluster;
using Aquiles.Core.Configuration;
using Aquiles.Core.Cluster.Impl;
using Aquiles.Core.Cluster.Factory;
using Aquiles.Core.Connection.Pooling;
using Aquiles.Core.Connection.Pooling.Factory;
using Aquiles.Core.Connection.Factory;
using Aquiles.Core.Connection.EndpointManager;
using Aquiles.Core.Connection.EndpointManager.Factory;
using Aquiles.Core.Model;
using Aquiles.Core.Model.Impl;
using Aquiles.Core.Diagnostics;

namespace Aquiles.Core.Configuration
{
    public abstract class AbstractAquilesClusterBuilder
    {
        public enum ClusterType
        {
            DEFAULT,
        }

        /// <summary>
        /// Type of ConnectionPool
        /// </summary>
        public enum PoolType
        {
            /// <summary>
            /// No pool is used. Clients are created based on need, they are disposed as soon as they are returned.
            /// </summary>
            NOPOOL = 0,
            /// <summary>
            /// Warmup enable and size-controlled enabled pool.
            /// </summary>
            SIZECONTROLLEDPOOL = 1

        }

        public enum EndpointManagerType
        {
            /// <summary>
            /// Cycle through the list of endpoints to balance the pool connections
            /// </summary>
            ROUNDROBIN,
        }

        protected const string ENDPOINTMANAGER_DUETIME_KEY = "endpointManagerDueTime";
        protected const string ENDPOINTMANAGER_PERIODICTIME_KEY = "endpointManagerPeriodicTime";
        protected const string POOL_DUETIME_KEY = "poolDueTime";
        protected const string POOL_PERIODICTIME_KEY = "poolPeriodicTime";
        protected const string POOL_MINIMUM_CLIENTS_TO_KEEP_KEY = "minimumClientsToKeepInPool";
        protected const string POOL_MAXIMUM_CLIENTS_TO_SUPPORT_KEY = "maximumClientsToSupportInPool";
        protected const string POOL_MAGIC_NUMBER_KEY = "magicNumber";
        protected const string POOL_MAXIMUM_RETRIES_TO_POLL_CLIENT = "maximumRetriesToPollClient";


        public virtual ICluster Build(CassandraClusterElement clusterConfig, ILogger logger)
        {
            ICluster cluster = null;
            ClusterType clusterType;
            clusterType = (ClusterType) Enum.Parse(typeof(ClusterType), clusterConfig.ClusterType, true);
            switch (clusterType)
            {
                case ClusterType.DEFAULT:
                    cluster = this.BuildDefaultCluster(clusterConfig, logger);
                    break;
                default:
                    throw new NotImplementedException(String.Format("ClusterType '{0}' not implemented.", clusterType));
            }
            return cluster;
        }

        protected virtual ICluster BuildDefaultCluster(CassandraClusterElement clusterConfig, ILogger logger)
        {
            DefaultClusterFactory clusterFactory = new DefaultClusterFactory();
            clusterFactory.Logger = logger;
            clusterFactory.FriendlyName = clusterConfig.FriendlyName;
            clusterFactory.PoolManager = this.buildPoolManager(clusterConfig, logger, clusterFactory.FriendlyName);
            return clusterFactory.Create();
        }

        protected virtual IClientPool buildPoolManager(CassandraClusterElement clusterConfig, ILogger logger, string clusterName)
        {
            IClientPool clientPool = null;
            ConnectionElement connectionConfig = clusterConfig.Connection;
            PoolType poolType;
            poolType = (PoolType) Enum.Parse(typeof(PoolType), connectionConfig.PoolType, true);
            switch (poolType)
            {
                case PoolType.NOPOOL:
                    clientPool = this.buildNoClientPool(clusterConfig, logger, clusterName);
                    break;
                case PoolType.SIZECONTROLLEDPOOL:
                    clientPool = this.buildSizeControlledClientPool(clusterConfig, logger, clusterName);
                    break;
                default:
                    throw new NotImplementedException(String.Format("PoolType '{0}' not implemented.", poolType));
            }

            return clientPool;
        }

        private IClientPool buildSizeControlledClientPool(CassandraClusterElement clusterConfig, ILogger logger, string clusterName)
        {
            SpecialConnectionParameterElement specialConfig = null;
            int intTempValue = 0;
            SizeControlledClientPoolFactory poolFactory = new SizeControlledClientPoolFactory();
            poolFactory.Name = String.Concat(clusterName, "_sizeControlledPool");
            poolFactory.ClientFactory = this.buildClientFactory(clusterConfig);
            poolFactory.EndpointManager = this.buildEndpointManager(clusterConfig, logger, poolFactory.Name);
            specialConfig = this.retrieveSpecialParameter(clusterConfig.Connection.SpecialConnectionParameters, POOL_DUETIME_KEY);
            if (specialConfig != null && Int32.TryParse(specialConfig.Value, out intTempValue) && intTempValue >= 0)
            {
                poolFactory.DueTime = intTempValue;
            }
            specialConfig = this.retrieveSpecialParameter(clusterConfig.Connection.SpecialConnectionParameters, POOL_PERIODICTIME_KEY);
            if (specialConfig != null && Int32.TryParse(specialConfig.Value, out intTempValue) && intTempValue > 0)
            {
                poolFactory.PeriodicTime = intTempValue;
            }
            specialConfig = this.retrieveSpecialParameter(clusterConfig.Connection.SpecialConnectionParameters, POOL_MAGIC_NUMBER_KEY);
            if (specialConfig != null && Int32.TryParse(specialConfig.Value, out intTempValue) && intTempValue > 0)
            {
                poolFactory.MagicNumber = intTempValue;
            }
            specialConfig = this.retrieveSpecialParameter(clusterConfig.Connection.SpecialConnectionParameters, POOL_MAXIMUM_CLIENTS_TO_SUPPORT_KEY);
            if (specialConfig != null && Int32.TryParse(specialConfig.Value, out intTempValue) && intTempValue > 0)
            {
                poolFactory.MaximumClientsToSupport = intTempValue;
            }

            specialConfig = this.retrieveSpecialParameter(clusterConfig.Connection.SpecialConnectionParameters, POOL_MAXIMUM_RETRIES_TO_POLL_CLIENT);
            if (specialConfig != null && Int32.TryParse(specialConfig.Value, out intTempValue) && intTempValue >= 0)
            {
                poolFactory.MaximumRetriesToPollClient = intTempValue;
            }

            specialConfig = this.retrieveSpecialParameter(clusterConfig.Connection.SpecialConnectionParameters, POOL_MINIMUM_CLIENTS_TO_KEEP_KEY);
            if (specialConfig != null && Int32.TryParse(specialConfig.Value, out intTempValue) && intTempValue >= 0)
            {
                poolFactory.MinimumClientsToKeep = intTempValue;
            }

            poolFactory.Logger = logger;
                 
            return poolFactory.Create();
        }

        protected virtual IEndpointManager buildEndpointManager(CassandraClusterElement clusterConfig, ILogger logger, string poolName)
        {
            IEndpointManager endpointManager = null;
            EndpointManagerType endpointManagerType;
            endpointManagerType = (EndpointManagerType) Enum.Parse(typeof(EndpointManagerType), clusterConfig.EndpointManager.Type, true);
            switch (endpointManagerType)
            {
                case EndpointManagerType.ROUNDROBIN:
                    endpointManager = this.buildRoundRobinEndpointManager(clusterConfig, logger, poolName);
                    break;
                default:
                    throw new NotImplementedException(String.Format("EndpointManagerType '{0}' not implemented.", endpointManagerType));
            }
            return endpointManager;
        }

        private IEndpointManager buildRoundRobinEndpointManager(CassandraClusterElement clusterConfig, ILogger logger, string poolName)
        {
            RoundRobinEndpointManagerFactory endpointManagerFactory = new RoundRobinEndpointManagerFactory();
            endpointManagerFactory.Name = String.Concat(poolName,"_endpointManager");
            endpointManagerFactory.ClientFactory = this.buildClientFactory(clusterConfig);
            endpointManagerFactory.Endpoints = this.buildEndpoints(clusterConfig.EndpointManager.CassandraEndpoints, clusterConfig.EndpointManager.DefaultTimeout);
            SpecialConnectionParameterElement specialConfig = this.retrieveSpecialParameter(clusterConfig.Connection.SpecialConnectionParameters, ENDPOINTMANAGER_PERIODICTIME_KEY);
            int intTempValue = 0;
            if (specialConfig != null && Int32.TryParse(specialConfig.Value, out intTempValue))
            {
                endpointManagerFactory.DueTime = intTempValue;
            }
            specialConfig = this.retrieveSpecialParameter(clusterConfig.Connection.SpecialConnectionParameters, ENDPOINTMANAGER_PERIODICTIME_KEY);
            if (specialConfig != null && Int32.TryParse(specialConfig.Value, out intTempValue))
            {
                endpointManagerFactory.PeriodicTime = intTempValue;
            }

            endpointManagerFactory.Logger = logger;

            return endpointManagerFactory.Create();
        }

        protected virtual List<IEndpoint> buildEndpoints(CassandraEndpointCollection cassandraEndpointCollection, int defaultTimeout)
        {
            List<IEndpoint> endpoints = new List<IEndpoint>();
            IEndpoint endpoint = null;
            foreach(CassandraEndpointElement endpointConfig in cassandraEndpointCollection) 
            {
                endpoint = this.buildEndpoint(endpointConfig, defaultTimeout);
                endpoints.Add(endpoint);
            }

            return endpoints;
        }

        protected abstract IConnectionFactory buildClientFactory(CassandraClusterElement clusterConfig);

        protected virtual IEndpoint buildEndpoint(CassandraEndpointElement endpointConfig, int defaultTimeout)
        {
            DefaultEndpoint endpoint = new DefaultEndpoint();
            endpoint.Address = endpointConfig.Address;
            endpoint.Port = endpointConfig.Port;
            endpoint.UpDateTime = DateTime.Now;
            endpoint.Timeout = (endpointConfig.Timeout != 0) ? endpointConfig.Timeout : defaultTimeout;

            return endpoint;
        }

        private IClientPool buildNoClientPool(CassandraClusterElement clusterConfig, ILogger logger, string clusterName)
        {
            NoClientPoolFactory poolFactory = new NoClientPoolFactory();
            poolFactory.Name = String.Concat(clusterName, "_noPool");
            poolFactory.ClientFactory = this.buildClientFactory(clusterConfig);
            poolFactory.EndpointManager = this.buildEndpointManager(clusterConfig, logger, poolFactory.Name);
            poolFactory.Logger = logger;

            return poolFactory.Create();
        }

        protected SpecialConnectionParameterElement retrieveSpecialParameter(SpecialConnectionParameterCollection specialConnectionParameterCollection, string propertyKey)
        {
            SpecialConnectionParameterElement element = null;
            if (specialConnectionParameterCollection != null)
            {
                element = specialConnectionParameterCollection[propertyKey];
            }
            return element;
        }

    }
}
