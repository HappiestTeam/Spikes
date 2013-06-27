using System;
using System.Collections.Generic;

using System.Text;
using System.Configuration;

namespace Aquiles.Core.Configuration
{
    /// <summary>
    /// Configuration Element holding special connection parameters
    /// </summary>
    public class SpecialConnectionParameterElement : ConfigurationElement
    {
        /// <summary>
        /// get or set the Key for the special connection parameters (must be unique in the collection)
        /// </summary>
        [ConfigurationProperty("key", IsRequired = true)]
        public string Key
        {
            get { return (string)this["key"]; }
            set { this["key"] = value; }
        }

        /// <summary>
        /// get or set the value for the special connection parameters 
        /// </summary>
        [ConfigurationProperty("value", IsRequired = true)]
        public string Value
        {
            get { return (string)this["value"]; }
            set { this["value"] = value; }
        }
    }
}
