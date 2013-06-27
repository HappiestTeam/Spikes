using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aquiles.Configuration;
using Aquiles.Core.Client;
using Aquiles.Core.Cluster;
using AquilesCoreHelper = Aquiles.Core.AquilesHelper;

namespace Aquiles.Cassandra10
{
    public sealed class AquilesHelper
    {
        private const string SECTION_CONFIGURATION_NAME = "aquilesConfiguration";
        private static bool clusterCreated;
        private static object syncObject;
 
        #region static
        private static AquilesCoreHelper _instance;
        static AquilesHelper()
        {
            clusterCreated = false;
            syncObject = new object();
        }

        /// <summary>
        /// Read the configuration section, create logger, create clusters
        /// <remarks>can throw <see cref="Aquiles.Core.Exceptions.AquilesException"/> in case something went wrong</remarks>
        /// </summary>
        public static void Initialize()
        {
            if (!clusterCreated)
            {
                CreateClusters();
            }
        }

        /// <summary>
        /// Retrieve a ICluster instance to work with.
        /// <remarks>can throw <see cref="Aquiles.Core.Exceptions.AquilesException"/> in case something went wrong</remarks>
        /// </summary>
        /// <param name="clusterName">friendly names chosen in the configuration section on the .config file</param>
        /// <returns>it returns a cluster instance.</returns>
        public static ICluster RetrieveCluster(string clusterName)
        {
            if (!clusterCreated)
            {
                CreateClusters();
            }
            return _instance.retrieveCluster(clusterName);
        }
        #endregion 

        private static void CreateClusters()
        {
            lock (syncObject)
            {
                if (!clusterCreated)
                {
                    AquilesClusterBuilder builder = new AquilesClusterBuilder();
                    _instance = new AquilesCoreHelper(builder, SECTION_CONFIGURATION_NAME);
                    clusterCreated = true;
                }
            }
        }
    }
}
