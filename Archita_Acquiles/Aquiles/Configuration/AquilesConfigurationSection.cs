using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace Aquiles.Configuration
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
        /// Gets a System.Configuration.SectionInformation object that contains the non-customizable information and functionality of the System.Configuration.ConfigurationSection object. 
        /// </summary>
        public new SectionInformation SectionInformation
        {
            get
            {
                SectionInformation modifiedSectionInformation = base.SectionInformation;
                // indicates where this section can be placed. In this case i allow it everywhere (machine.config, app.config, etc)
                modifiedSectionInformation.AllowDefinition = ConfigurationAllowDefinition.Everywhere;
                // indicates that this section can be placed on the lowest heriachical config (machine, app, user.config)
                modifiedSectionInformation.AllowExeDefinition = ConfigurationAllowExeDefinition.MachineToLocalUser;
                // indicates that this section can have a location attribute pointing to an external file
                modifiedSectionInformation.AllowLocation = true;
                // indicates that the section can have several definitions and depending the heriachical distribution it can be overrided.
                modifiedSectionInformation.AllowOverride = true;

                modifiedSectionInformation.ForceSave = false;
                // indicates that this section can be delcared in a web.config root and all child sites will inherit it.
                modifiedSectionInformation.InheritInChildApplications = true;
                // indicates that this section must appeared in the config file.
                modifiedSectionInformation.ForceDeclaration(true);
                // indicates that this section can have its values overriden
                modifiedSectionInformation.OverrideMode = OverrideMode.Inherit;
                // indicates that child sections inherited from this one has the overrideMode set to inherited in case there is no definition
                modifiedSectionInformation.OverrideModeDefault = OverrideMode.Inherit;

                return modifiedSectionInformation;
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
