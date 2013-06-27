using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apache.Cassandra;
using System.IO;
using Aquiles.Logging;
using Aquiles.Model;
using Aquiles.Exceptions;

namespace Aquiles.Command
{
    /// <summary>
    /// Command to insert a Column into a Keyspace of a given cluster
    /// </summary>
    public class InsertCommand : AbstractKeySpaceColumnFamilyKeyDependantCommand, IAquilesCommand
    {
        private LoggerHelper logger;
        /// <summary>
        /// get or set the name of the supercolumn
        /// </summary>
        public String SuperColumn
        {
            set;
            get;
        }

        /// <summary>
        /// get or set the column information
        /// </summary>
        public AquilesColumn Column
        {
            set;
            get;
        }

        /// <summary>
        /// ctor
        /// </summary>
        public InsertCommand() : base()
        {
            this.logger = LoggerHelper.CreateLogger(this.GetType());
        }

        #region IAquilesCommand Members
        /// <summary>
        /// Executes a "insert" over the connection. No return values.
        /// </summary>
        /// <param name="cassandraClient">opened Thrift client</param>
        public void Execute(Cassandra.Client cassandraClient)
        {
            logger.Debug(logger.StringFormatInvariantCulture("Adding key '{0}' from columnFamily '{1}' from KeySpace '{2}'.", this.Key, this.ColumnFamily, this.KeySpace));
            Column column = this.Column.ToThrift();
            ColumnPath columnPath = this.BuildColumnPath(this.ColumnFamily, this.SuperColumn, this.Column.ColumnName);

            cassandraClient.insert(this.KeySpace, this.Key, columnPath, column.Value, column.Timestamp, this.GetCassandraConsistencyLevel());

        }
        /// <summary>
        /// Validate the input parameters. 
        /// Throws <see cref="Aquiles.Exceptions.AquilesCommandParameterException"/>  in case there is some malformed or missing input parameters
        /// </summary>
        /// <param name="keyspaces">Dictionary of the keyspaces contained in the cluster that corresponds to the connection</param>
        public override void ValidateInput(Dictionary<String, AquilesKeyspace> keyspaces)
        {
            base.ValidateInput(keyspaces);
            if (this.Column == null)
            {
                throw new AquilesCommandParameterException("Column parameter must have a value.");
            }
            this.Column.ValidateForInsertOperation();
        }
        #endregion

    }
}
