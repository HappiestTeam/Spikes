using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apache.Cassandra;
using Aquiles.Model;
using Aquiles.Exceptions;

namespace Aquiles.Command
{
    /// <summary>
    /// Command to retrieve slices of data on the given key
    /// </summary>
    public class GetSliceCommand : AbstractKeySpaceColumnFamilyKeyDependantCommand, IAquilesCommand
    {
        /// <summary>
        /// get or set the name of the SuperColumn
        /// </summary>
        public string SuperColumn
        {
            get;
            set;
        }

        /// <summary>
        /// get or set the predicate to use
        /// </summary>
        public AquilesSlicePredicate Predicate
        {
            get;
            set;
        }

        /// <summary>
        /// get the output of the command
        /// </summary>
        public Out Output
        {
            get;
            private set;
        }

        #region IAquilesCommand Members
        /// <summary>
        /// Executes a "get_slice" over the connection. No return values.
        /// </summary>
        /// <param name="cassandraClient">opened Thrift client</param>
        public void Execute(Cassandra.Client cassandraClient)
        {
            this.Output = null;
            ColumnParent columnParent = this.BuildColumnParent(this.SuperColumn);
            SlicePredicate predicate = this.Predicate.ToThrift();
            List<ColumnOrSuperColumn> output = cassandraClient.get_slice(this.KeySpace, this.Key, columnParent, predicate, this.GetCassandraConsistencyLevel());
            this.buildOut(output);
        }

        /// <summary>
        /// Validate the input parameters. 
        /// Throws <see cref="Aquiles.Exceptions.AquilesCommandParameterException"/>  in case there is some malformed or missing input parameters
        /// </summary>
        /// <param name="keyspaces">Dictionary of the keyspaces contained in the cluster that corresponds to the connection</param>
        public override void ValidateInput(Dictionary<string, AquilesKeyspace> keyspaces)
        {
            base.ValidateInput(keyspaces);
            AquilesKeyspace keyspace = keyspaces[this.KeySpace];
            AquilesColumnFamily destinationColumnFamily = keyspace.ColumnFamilies[this.ColumnFamily];

            //TODO reveer esta validación :)
            //if (String.IsNullOrEmpty(this.SuperColumn) && destinationColumnFamily.Type == AquilesColumnFamilyType.Super)
            //{
            //    throw new AquilesCommandParameterException("SuperColumn cannot be null or empty when accesing a Super type ColumnFamily.");
            //}
            //else if (!String.IsNullOrEmpty(this.SuperColumn) && destinationColumnFamily.Type == AquilesColumnFamilyType.Standard)
            //{
            //    throw new AquilesCommandParameterException("SuperColumn must be be null or empty when accesing a Standard type ColumnFamily.");
            //}

            if (this.Predicate == null)
            {
                throw new AquilesCommandParameterException("Predicate cannot be null.");
            }
            else
            {
                this.Predicate.ValidateForQueryOperation();
            }
        }
        #endregion

        private void buildOut(List<ColumnOrSuperColumn> output)
        {
            this.Output = new Out();
            this.Output.Results = new List<GetCommand.Out>();
            foreach (ColumnOrSuperColumn columnOrSuperColumn in output)
            {
                this.Output.Results.Add(new GetCommand.Out()
                    {
                        Column = this.BuildColumn(columnOrSuperColumn.Column),
                        SuperColumn = this.buildSuperColumn(columnOrSuperColumn.Super_column)
                    });
            }
        }

        private AquilesSuperColumn buildSuperColumn(SuperColumn superColumn)
        {
            AquilesSuperColumn aquilesSuperColumn = null;
            if (superColumn != null)
            {
                aquilesSuperColumn = new AquilesSuperColumn();
                aquilesSuperColumn.Name = ByteEncoderHelper.FromByteArray(superColumn.Name);
                aquilesSuperColumn.Columns = new List<AquilesColumn>(superColumn.Columns.Count);
                foreach (Column column in superColumn.Columns)
                {
                    aquilesSuperColumn.Columns.Add(this.BuildColumn(column));
                }
            }
            return aquilesSuperColumn;
        }

        private AquilesColumn BuildColumn(Column column)
        {
            AquilesColumn aquilesColumn = null;
            if (column != null)
            {
                aquilesColumn = new AquilesColumn();
                aquilesColumn.ColumnName = ByteEncoderHelper.FromByteArray(column.Name);
                aquilesColumn.Timestamp = column.Timestamp;
                aquilesColumn.Value = ByteEncoderHelper.FromByteArray(column.Value);
            }
            return aquilesColumn;
        }

        /// <summary>
        /// structure to Return Values
        /// </summary>
        public class Out
        {

            /// <summary>
            /// get or set the results 
            /// </summary>
            public List<GetCommand.Out> Results
            {
                get;
                set;
            }

        }

    }
}
