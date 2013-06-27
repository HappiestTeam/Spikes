using System;
using System.Collections.Generic;

using System.Text;
using System.Configuration;

namespace Aquiles.Core.Configuration
{
    /// <summary>
    /// ConfigurationSection for Aquiles
    /// </summary>
    public class AquilesConfigurationSection : ConfigurationSection
    {
        /// <summary>
        /// ctor
        /// </summary>
        protected AquilesConfigurationSection() : base() { }

        /// <summary>
        /// get or set the type of the client class to use for logging purpose
        /// </summary>
        [ConfigurationProperty("monitorFeaturesType", IsRequired = false)]
        public AquilesTextElement MonitorFeaturesType
        {
            get
            {
                return (AquilesTextElement)this["monitorFeaturesType"];
            }
            set
            {
                this["monitorFeaturesType"] = value;
            }
        }

        /// <summary>
        /// get or set the type of the client class to use for logging purpose
        /// </summary>
        [ConfigurationProperty("loggingManager", IsRequired = false)]
        public AquilesTextElement LoggingManager
        {
            get
            {
                return (AquilesTextElement)this["loggingManager"];
            }
            set
            {
                this["loggingManager"] = value;
            }
        }

        /// <summary>
        /// get or set the collection of clusters
        /// </summary>
        [ConfigurationProperty("clusters", IsRequired = true)]
        [ConfigurationCollection(typeof(CassandraClusterElement), AddItemName = "add", RemoveItemName = "remove", ClearItemsName = "clear")]
        public CassandraClusterCollection CassandraClusters
        {
            get
            {
                return (CassandraClusterCollection)this["clusters"];
            }
            set
            {
                this["clusters"] = value;
            }
        }
    }
}
