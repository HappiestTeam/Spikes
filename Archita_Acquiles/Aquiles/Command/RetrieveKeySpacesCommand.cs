using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Thrift.Collections;
using Apache.Cassandra;
using Aquiles.Model;

namespace Aquiles.Command
{
    /// <summary>
    /// Command to retrieve the list of keyspaces for a cluster
    /// </summary>
    public class RetrieveKeySpacesCommand : IAquilesCommand
    {
        /// <summary>
        /// get the list of keyspaces
        /// </summary>
        public HashSet<string> KeySpaces
        {
            get;
            private set;
        }

        #region IAquilesCommand Members
        /// <summary>
        /// Executes a "describe_keyspaces" over the connection, set the KeySpaces property with the returned value.
        /// </summary>
        /// <param name="cassandraClient">opened Thrift client</param>
        public void Execute(Cassandra.Client cassandraClient)
        {
            THashSet<string> keySpaces = cassandraClient.describe_keyspaces();
            this.KeySpaces = new HashSet<string>(keySpaces);
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
