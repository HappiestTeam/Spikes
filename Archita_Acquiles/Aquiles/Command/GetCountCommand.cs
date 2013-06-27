using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apache.Cassandra;
using Aquiles.Model;

namespace Aquiles.Command
{
    /// <summary>
    /// Command to retrieve a Column or SuperColumn from Keyspace of a given cluster with the given key
    /// </summary>
    public class GetCountCommand : AbstractKeySpaceColumnFamilyKeyDependantCommand, IAquilesCommand
    {
        /// <summary>
        /// get or set the SuperColumn
        /// </summary>
        public string SuperColumnName
        {
            set;
            get;
        }

        /// <summary>
        /// get the count retrieved after command execution
        /// </summary>
        public int Count
        {
            get;
            private set;
        }

        #region IAquilesCommand Members
        /// <summary>
        /// Executes a "get_count" over the connection. Return values are set into Output
        /// </summary>
        /// <param name="cassandraClient">opened Thrift client</param>
        public void Execute(Cassandra.Client cassandraClient)
        {
            ColumnParent columnParent = this.BuildColumnParent(this.SuperColumnName);
            this.Count = cassandraClient.get_count(this.KeySpace, this.Key, columnParent, this.GetCassandraConsistencyLevel());
        }
        /// <summary>
        /// Validate the input parameters. 
        /// Throws <see cref="Aquiles.Exceptions.AquilesCommandParameterException"/>  in case there is some malformed or missing input parameters
        /// </summary>
        /// <param name="keyspaces">Dictionary of the keyspaces contained in the cluster that corresponds to the connection</param>
        public override void ValidateInput(Dictionary<string, AquilesKeyspace> keyspaces)
        {
            base.ValidateInput(keyspaces);
        }
        #endregion
    }
}
