using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aquiles.Model;

namespace Aquiles.Command
{
    /// <summary>
    /// Command to retrieve the ClusterName from a Cluster (the real name)
    /// </summary>
    public class RetrieveClusterNameCommand : IAquilesCommand
    {
        /// <summary>
        /// Get the ClusterName
        /// </summary>
        public string ClusterName
        {
            get;
            private set;
        }

        #region IAquilesCommand Members

        /// <summary>
        /// Executes a "describe_cluster_name" over the connection, set the ClusterName property with the returned value.
        /// </summary>
        /// <param name="cassandraClient">opened Thrift client</param>
        public void Execute(Apache.Cassandra.Cassandra.Client cassandraClient)
        {
            this.ClusterName = cassandraClient.describe_cluster_name();
        }

        /// <summary>
        /// Validate the input parameters. 
        /// Throws <see cref="Aquiles.Exceptions.AquilesCommandParameterException"/>  in case there is some malformed or missing input parameters
        /// </summary>
        /// <param name="keyspaces">Dictionary of the keyspaces contained in the cluster that corresponds to the connection</param>
        public void ValidateInput(Dictionary<String,AquilesKeyspace> keyspaces)
        {
            //do nothing
        }

        #endregion
    }
}
