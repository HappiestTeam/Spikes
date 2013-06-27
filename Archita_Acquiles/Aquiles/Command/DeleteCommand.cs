using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aquiles.Logging;
using Apache.Cassandra;
using Aquiles.Model;
using Aquiles.Exceptions;

namespace Aquiles.Command
{
    /// <summary>
    /// Command to delete a Column, SuperColumn or a Key from Keyspace of a given cluster
    /// </summary>
    public class DeleteCommand : AbstractKeySpaceColumnFamilyKeyDependantCommand, IAquilesCommand
    {
        private LoggerHelper logger;

        /// <summary>
        /// get or set supercolumn name
        /// </summary>
        public string SuperColumnName
        {
            set;
            get;
        }
        /// <summary>
        /// get or set Column information
        /// </summary>
        public AquilesColumn Column
        {
            set;
            get;
        }

        /// <summary>
        /// ctor
        /// </summary>
        public DeleteCommand() : base()
        {
            this.logger = LoggerHelper.CreateLogger(this.GetType());
        }

        #region IAquilesCommand Members
        /// <summary>
        /// Executes a "remove" over the connection. No return values
        /// </summary>
        /// <param name="cassandraClient">opened Thrift client</param>
        public void Execute(Cassandra.Client cassandraClient)
        {
            ColumnPath columnPath;
            if (this.Column != null)
            {
                logger.Debug(logger.StringFormatInvariantCulture("Removing column '{0}' for key '{1}' from columnFamily '{2}' from KeySpace '{3}'.", this.Column.ColumnName, this.Key, this.ColumnFamily, this.KeySpace));
                columnPath = this.BuildColumnPath(this.ColumnFamily, this.SuperColumnName, this.Column.ColumnName);
                cassandraClient.remove(this.KeySpace, this.Key, columnPath, this.Column.Timestamp.Value, this.GetCassandraConsistencyLevel());
            }
            else
            {
                logger.Debug(logger.StringFormatInvariantCulture("Removing key '{0}' from columnFamily '{1}' from KeySpace '{2}'.", this.Key, this.ColumnFamily, this.KeySpace));
                columnPath = this.BuildColumnPath(this.ColumnFamily, this.SuperColumnName, null);
                cassandraClient.remove(this.KeySpace, this.Key, columnPath, UnixHelper.UnixTimestamp, this.GetCassandraConsistencyLevel());
            }
        }

        /// <summary>
        /// Validate the input parameters. 
        /// Throws <see cref="Aquiles.Exceptions.AquilesCommandParameterException"/>  in case there is some malformed or missing input parameters
        /// </summary>
        /// <param name="keyspaces">Dictionary of the keyspaces contained in the cluster that corresponds to the connection</param>
        public override void ValidateInput(Dictionary<String,AquilesKeyspace> keyspaces)
        {
            base.ValidateInput(keyspaces);
            if (this.Column != null)
            {
                this.Column.ValidateForDeletationOperation();
                if (this.Column.Timestamp == null)
                {
                    throw new AquilesCommandParameterException("If input Column exists, must have a valid Timestamp.");
                }
            }
        }
        #endregion
    }
}
