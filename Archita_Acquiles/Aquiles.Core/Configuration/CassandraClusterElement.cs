using System;
using System.Collections.Generic;

using System.Text;
using System.Configuration;

namespace Aquiles.Core.Configuration
{
    /// <summary>
    /// ConfigurationElement holding Cluster information
    /// </summary>
    public class CassandraClusterElement : ConfigurationElement
    {

        /// <summary>
        /// get or set the Friendly name
        /// </summary>
        [ConfigurationProperty("friendlyName", DefaultValue = " ", IsRequired = true)]
        [StringValidator(MinLength = 1, MaxLength = int.MaxValue)]
        public string FriendlyName
        {
            get { return (string)this["friendlyName"]; }
            set { this["friendlyName"] = value; }
        }

        /// <summary>
        /// get or set the Friendly name
        /// </summary>
        [ConfigurationProperty("clusterType", DefaultValue = "DEFAULT", IsRequired = false)]
        [StringValidator(MinLength = 1, MaxLength = int.MaxValue)]
        public string ClusterType
        {
            get { return (string)this["clusterType"]; }
            set { this["clusterType"] = value; }
        }

        /// <summary>
        /// get or set the Connection configuration
        /// </summary>
        [ConfigurationProperty("connection", IsRequired = false)]
        public ConnectionElement Connection
        {
            get
            {
                return (ConnectionElement)this["connection"];
            }
            set
            {
                this["connection"] = value;
            }
        }

        /// <summary>
        /// get or set the endpoint manager configuration
        /// </summary>
        [ConfigurationProperty("endpointManager", IsRequired = true)]
        public EndpointManagerElement EndpointManager
        {
            get
            {
                return (EndpointManagerElement)this["endpointManager"];
            }
            set
            {
                this["endpointManager"] = value;
            }
        }
    }
}
