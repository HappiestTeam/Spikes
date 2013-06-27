using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apache.Cassandra;
using Aquiles.Model;
using Aquiles.Logging;
using Aquiles.Exceptions;

namespace Aquiles.Command
{
    /// <summary>
    /// Command to retrieve a Column or SuperColumn from Keyspace of a given cluster with the given key
    /// </summary>
    public class GetCommand : AbstractKeySpaceColumnFamilyKeyDependantCommand, IAquilesCommand
    {
        private LoggerHelper logger;

        /// <summary>
        /// get the return value
        /// </summary>
        public Out Output
        {
            get;
            private set;
        }

        /// <summary>
        /// get or set the SuperColumn
        /// </summary>
        public string SuperColumnName
        {
            set;
            get;
        }

        /// <summary>
        /// get or set the ColumnName
        /// </summary>
        public string ColumnName
        {
            set;
            get;
        }

        /// <summary>
        /// ctor
        /// </summary>
        public GetCommand() : base()
        {
            logger = LoggerHelper.CreateLogger(this.GetType());
        }


        #region IAquilesCommand Members
        /// <summary>
        /// Executes a "get" over the connection. Return values are set into Output
        /// </summary>
        /// <param name="cassandraClient">opened Thrift client</param>
        public void Execute(Cassandra.Client cassandraClient)
        {
            ColumnOrSuperColumn columnOrSupercolumn = null;
            ColumnPath columnPath = this.BuildColumnPath(this.ColumnFamily, this.SuperColumnName, this.ColumnName);
            try
            {
                columnOrSupercolumn = cassandraClient.get(this.KeySpace, this.Key, columnPath, this.GetCassandraConsistencyLevel());
            }
            catch (NotFoundException ex)
            {
                logger.Warn(
                    logger.StringFormatInvariantCulture("{{ KeySpace: '{0}', Key: '{1}', ColumnFamily: '{2}', SuperColumn: '{3}', Column: '{4}' }} not found.", 
                        this.KeySpace, 
                        this.Key, 
                        this.ColumnFamily, 
                        (this.SuperColumnName != null) ? this.SuperColumnName : String.Empty,
                        (this.ColumnName != null) ? this.ColumnName : String.Empty), 
                    ex);
            }

            if (columnOrSupercolumn != null)
            {
                this.buildOut(columnOrSupercolumn);
            }
            else
            {
                // in case of reuse of the command
                this.Output = null;
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
        }
        #endregion

        private void buildOut(ColumnOrSuperColumn columnOrSupercolumn)
        {
            Out outResponse = new Out();
            if (columnOrSupercolumn.Column != null)
            {
                outResponse.Column = this.buildAquilesColumn(columnOrSupercolumn.Column);
            }
            if (columnOrSupercolumn.Super_column != null)
            {
                outResponse.SuperColumn = this.buildAquilesSuperColumn(columnOrSupercolumn.Super_column);
            }
            this.Output = outResponse;
        }

        private AquilesSuperColumn buildAquilesSuperColumn(SuperColumn superColumn)
        {
            AquilesSuperColumn aquilesSuperColumn = new AquilesSuperColumn();
            aquilesSuperColumn.Name = ByteEncoderHelper.FromByteArray(superColumn.Name);
            if (superColumn.Columns != null)
            {
                foreach (Column column in superColumn.Columns)
                {
                    aquilesSuperColumn.Columns.Add(this.buildAquilesColumn(column));
                }
            }
            return aquilesSuperColumn;
        }

        private AquilesColumn buildAquilesColumn(Column column)
        {
            AquilesColumn aquilesColumn = new AquilesColumn();
            aquilesColumn.ColumnName = ByteEncoderHelper.FromByteArray(column.Name);
            aquilesColumn.Timestamp = column.Timestamp;
            aquilesColumn.Value = ByteEncoderHelper.FromByteArray(column.Value);
            return aquilesColumn;
        }

        /// <summary>
        /// structure to Return Values
        /// </summary>
        public class Out {
            /// <summary>
            /// get or set Column Information
            /// </summary>
            public AquilesColumn Column
            {
                get;
                set;
            }
            /// <summary>
            /// get or set SuperColumn Information
            /// </summary>
            public AquilesSuperColumn SuperColumn
            {
                get;
                set;
            }
        }



    }
}
