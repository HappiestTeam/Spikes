using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Collections.Specialized;
using Aquiles.Connection.Pooling;
using Aquiles.Connection.Factory;

namespace Aquiles.Configuration
{
    /// <summary>
    /// Configuration Element holding connection information
    /// </summary>
    public class ConnectionElement : ConfigurationElement
    {
        /// <summary>
        /// get or set pool Type
        /// </summary>
        [ConfigurationProperty("poolType", DefaultValue="NOPOOL", IsRequired = true)]
        public ConnectionPoolType PoolType
        {
            get
            {
                return (ConnectionPoolType) this["poolType"];
            }
            set
            {
                this["poolType"] = value;
            }
        }

        /// <summary>
        /// get or set factory type
        /// </summary>
        [ConfigurationProperty("factoryType", DefaultValue = "BUFFERED", IsRequired = true)]
        public ConnectionFactoryType FactoryType
        {
            get
            {
                return (ConnectionFactoryType)this["factoryType"];
            }
            set
            {
                this["factoryType"] = value;
            }
        }

        /// <summary>
        /// get or set collection of special parameters
        /// </summary>
        [ConfigurationProperty("specialConnectionParameters", IsRequired = false)]
        [ConfigurationCollection(typeof(SpecialConnectionParameterElement), AddItemName = "add", RemoveItemName = "remove", ClearItemsName = "clear")]
        public SpecialConnectionParameterCollection SpecialConnectionParameters
        {
            get
            {
                return (SpecialConnectionParameterCollection)this["specialConnectionParameters"];
            }
            set
            {
                this["cassandraEndpoints"] = value;
            }
        }

    }
}
