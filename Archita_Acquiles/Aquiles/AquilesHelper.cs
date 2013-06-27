using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aquiles.Logging;
using Aquiles.Configuration;
using System.Configuration;
using Aquiles.Impl;
using System.Runtime.CompilerServices;
using Aquiles.Exceptions;
using System.Globalization;

namespace Aquiles
{
    /// <summary>
    /// Static class to use as entrypoint for Aquiles. 
    /// Responsability: 
    ///     * Manage cluster configuration
    ///     * Manage connection creation
    /// </summary>
    public class AquilesHelper
    {
        private static AquilesHelper _instance;
        private LoggerHelper logger;
        private AquilesConfigurationSection configuration;
        private Dictionary<String, IAquilesCluster> clusters;

        static AquilesHelper()
        {
            AquilesHelper a = new AquilesHelper();
            _instance = a;
        }

        /// <summary>
        /// private instance constructor that calls Initialize
        /// </summary>
        private AquilesHelper()
        {
            this.InstanceInitialize();
        }

        /// <summary>
        /// Read the configuration section, create logger, create clusters
        /// <remarks>can throw <see cref="Aquiles.Exceptions.AquilesException"/> in case something went wrong</remarks>
        /// </summary>
        private void InstanceInitialize()
        {
            long startedTicks = DateTime.Now.Ticks;
            this.configuration = (AquilesConfigurationSection)ConfigurationManager.GetSection("aquilesConfiguration");
            if (this.configuration != null)
            {
                CreateLogger();
                logger = LoggerHelper.CreateLogger(typeof(AquilesHelper));

                CreateClusters();
                long durationTicks = DateTime.Now.Ticks - startedTicks;
                TimeSpan a = new TimeSpan(durationTicks);
                logger.Info(logger.StringFormatInvariantCulture("Startup took {0} ms.", a.Milliseconds));
            }
            else
            {
                throw new AquilesException("Configuration not loaded.");
            }
        }

        /// <summary>
        /// Create clusters within configuration
        /// </summary>
        private void CreateClusters()
        {
            clusters = new Dictionary<string, IAquilesCluster>();
            foreach (CassandraClusterElement clusterInfo in configuration.CassandraClusters)
            {
                IAquilesCluster cluster = new DefaultAquilesCluster(clusterInfo);
                cluster.Initialize();
                clusters.Add(cluster.FriendlyName, cluster);
            }
        }

        /// <summary>
        /// Create an instance of the logger type from config file
        /// <remarks>can throw <see cref="Aquiles.Configuration.AquilesConfigurationException"/> in case something went wrong</remarks>
        /// </summary>
        private void CreateLogger()
        {

            if (this.configuration.LoggingManager != null && !String.IsNullOrEmpty(this.configuration.LoggingManager.Text))
            {
                Type loggerType = Type.GetType(configuration.LoggingManager.Text);
                if (loggerType != null)
                {
                    LoggerHelper.CreateBridge(loggerType);
                }
                else
                {
                    throw new AquilesConfigurationException("Logger Type cannot be found.");
                }
            }
        }

        /// <summary>
        /// Read the configuration section, create logger, create clusters
        /// <remarks>can throw <see cref="Aquiles.Exceptions.AquilesException"/> in case something went wrong</remarks>
        /// </summary>
        public static void Initialize() 
        {
            // do nothing, this is a trick for user clients
        }

        /// <summary>
        /// Retrieve a connection for cluster associated with the given clusterName. In case there is no cluster configured with the friendly name given, a null is returned.
        /// <remarks>can throw <see cref="Aquiles.Exceptions.AquilesException"/> in case something went wrong</remarks>
        /// </summary>
        /// <param name="clusterName">friendly names chosen in the configuration section on the .config file</param>
        /// <returns>it returns a connection to work against the cluster.</returns>
        public static IAquilesConnection RetrieveConnection(String clusterName)
        {
            return _instance.InstanceRetrieveConnection(clusterName);
        }

        /// <summary>
        /// Retrieve a connection for cluster associated with the given clusterName. In case there is no cluster configured with the friendly name given, a null is returned.
        /// <remarks>can throw <see cref="Aquiles.Exceptions.AquilesException"/> in case something went wrong</remarks>
        /// </summary>
        /// <param name="clusterName">friendly names chosen in the configuration section on the .config file</param>
        /// <returns>it returns a connection to work against the cluster.</returns>
        private IAquilesConnection InstanceRetrieveConnection(String clusterName)
        {
            if (clusterName != null)
            {
                IAquilesCluster cluster = null;
                DefaultAquilesConnection connection = null;
                if (clusters.TryGetValue(clusterName, out cluster))
                {
                    connection = new DefaultAquilesConnection(cluster);
                }
                else
                {
                    throw new AquilesException(String.Format(CultureInfo.CurrentCulture, "No cluster found with name '{0}'", clusterName));
                }
                return connection;
            }
            else
            {
                throw new ArgumentNullException("clusterName");
            }
        }
    }
}
