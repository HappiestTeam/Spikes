using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aquiles.Core.Configuration;
using Aquiles.Core.Cluster;
using System.Configuration;
using Aquiles.Core.Client;
using Aquiles.Core.Exceptions;
using Aquiles.Core.Diagnostics;
using Aquiles.Core.Diagnostics.Impl;

namespace Aquiles.Core
{
    public sealed class AquilesHelper
    {
        public AquilesHelper(AbstractAquilesClusterBuilder builder, string sectionConfigurationName)
        {
            AquilesConfigurationSection section = (AquilesConfigurationSection)ConfigurationManager.GetSection(sectionConfigurationName);
            if (section != null)
            {
                ILogger logger = new TraceLogger();
                if (section.LoggingManager != null && section.LoggingManager.Text != null && !String.IsNullOrEmpty(section.LoggingManager.Text))
                {
                    // I got some logger configured
                    logger = this.CreateLogger(section.LoggingManager.Text);           
                }
                this.Clusters = this.buildClusters(builder, section, logger);
            }
            else
            {
                throw new AquilesConfigurationException("Configuration Section not found for '" + sectionConfigurationName + "'");
            }
        }

        public Dictionary<string, ICluster> Clusters
        {
            get;
            set;
        }

        public Dictionary<string, ICluster> buildClusters(AbstractAquilesClusterBuilder builder, AquilesConfigurationSection section, ILogger logger)
        {
            Dictionary<string, ICluster> clusters = null;
                CassandraClusterCollection clusterCollection = section.CassandraClusters;
                if (clusterCollection != null && clusterCollection.Count > 0)
                {
                    ICluster cluster = null;
                    clusters = new Dictionary<string, ICluster>(clusterCollection.Count);
                    foreach (CassandraClusterElement clusterConfig in section.CassandraClusters)
                    {
                        try
                        {
                            cluster = builder.Build(clusterConfig, logger);
                        }
                        catch (Exception e)
                        {
                            throw new AquilesConfigurationException("Exception found while creating cluster. See internal exception details.", e);
                        }
                        if (cluster != null)
                        {
                            if (!clusters.ContainsKey(cluster.Name))
                            {
                                clusters.Add(cluster.Name, cluster);
                            }
                        }
                    }
                }
                else
                {
                    throw new AquilesConfigurationException("Aquiles Configuration does not have any cluster configured.");
                }

            return clusters;
        }

        public ICluster retrieveCluster(string clusterName)
        {
            if (String.IsNullOrEmpty(clusterName))
                throw new ArgumentException("clusterName cannot be null nor empty");

            return this.Clusters[clusterName];
        }

        /// <summary>
        /// Create an instance of the logger type from config file
        /// <remarks>can throw <see cref="Aquiles.Configuration.AquilesConfigurationException"/> in case something went wrong</remarks>
        /// </summary>
        /// <param name="loggerTypeValue">String type of the logger to use</param>
        /// <returns>an instanciated ILogger</returns>
        private ILogger CreateLogger(string loggerTypeValue)
        {
            ILogger instance = null;
            Type loggerType = Type.GetType(loggerTypeValue);
            if (loggerType != null)
            {
                instance = (ILogger)Activator.CreateInstance(loggerType);
            }
            else
            {
                throw new AquilesConfigurationException("Logger Type cannot be found.");
            }
            return instance;
        }
    }
}
