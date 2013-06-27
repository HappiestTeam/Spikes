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
    /// Command to retrieve slices of data on each of the given keys in parallel
    /// </summary>                    
    public class GetKeyRangeSliceCommand : AbstractKeySpaceColumnFamilyDependantCommand, IAquilesCommand
    {
        /// <summary>
        /// get or set the Key / Token range to retrieve
        /// </summary>
        public IAquilesKeyTokenRange KeyTokenRange
        {
            get;
            set;
        }

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
        /// Executes a "get_range_slices" over the connection. No return values.
        /// </summary>
        /// <param name="cassandraClient">opened Thrift client</param>
        public void Execute(Cassandra.Client cassandraClient)
        {
            this.Output = null;
            ColumnParent columnParent = this.BuildColumnParent(this.SuperColumn);
            SlicePredicate predicate = this.Predicate.ToThrift();
            // Dictionary<string, List<ColumnOrSuperColumn>> output
            KeyRange keyRange = this.KeyTokenRange.ToThrift();
            List<KeySlice> result = cassandraClient.get_range_slices(this.KeySpace, columnParent, predicate, keyRange, this.GetCassandraConsistencyLevel());
            this.buildOut(result);
        }

        /// <summary>
        /// Validate the input parameters. 
        /// Throws <see cref="Aquiles.Exceptions.AquilesCommandParameterException"/>  in case there is some malformed or missing input parameters
        /// </summary>
        /// <param name="keyspaces">Dictionary of the keyspaces contained in the cluster that corresponds to the connection</param>
        public override void ValidateInput(Dictionary<string, AquilesKeyspace> keyspaces)
        {
            base.ValidateInput(keyspaces);

            if (this.KeyTokenRange == null)
            {
                throw new AquilesCommandParameterException("A KeyTokenRange must be supplied.");
            }

            this.KeyTokenRange.ValidateForQueryOperation();


            AquilesKeyspace keyspace = keyspaces[this.KeySpace];
            AquilesColumnFamily destinationColumnFamily = keyspace.ColumnFamilies[this.ColumnFamily];

            if (String.IsNullOrEmpty(this.SuperColumn) && destinationColumnFamily.Type == AquilesColumnFamilyType.Super)
            {
                throw new AquilesCommandParameterException("SuperColumn cannot be null or empty when accesing a Super type ColumnFamily.");
            }
            else if (!String.IsNullOrEmpty(this.SuperColumn) && destinationColumnFamily.Type == AquilesColumnFamilyType.Standard)
            {
                throw new AquilesCommandParameterException("SuperColumn must be be null or empty when accesing a Standard type ColumnFamily.");
            }

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

        private void buildOut(List<KeySlice> output)
        {
            List<GetCommand.Out> columnOrSuperColumnList = null;
            Out returnObj = new Out();
            returnObj.Results = new Dictionary<string, List<GetCommand.Out>>();
            
            foreach(KeySlice keySlice in output)
            {
                columnOrSuperColumnList = new List<GetCommand.Out>(keySlice.Columns.Count);
                foreach (ColumnOrSuperColumn columnOrSuperColumn in keySlice.Columns)
                {
                    columnOrSuperColumnList.Add(new GetCommand.Out()
                    {
                        Column = this.BuildColumn(columnOrSuperColumn.Column),
                        SuperColumn = this.buildSuperColumn(columnOrSuperColumn.Super_column)
                    });
                }
                returnObj.Results.Add(keySlice.Key, columnOrSuperColumnList);
            }

            this.Output = returnObj;
        }

        private AquilesSuperColumn buildSuperColumn(SuperColumn superColumn)
        {
            if (superColumn == null)
            {
                return null;
            }
            else
            {
                AquilesSuperColumn aquilesSuperColumn = new AquilesSuperColumn();
                aquilesSuperColumn.Name = ByteEncoderHelper.FromByteArray(superColumn.Name);
                aquilesSuperColumn.Columns = new List<AquilesColumn>(superColumn.Columns.Count);
                foreach (Column column in superColumn.Columns)
                {
                    aquilesSuperColumn.Columns.Add(this.BuildColumn(column));
                }

                return aquilesSuperColumn;
            }
        }

        private AquilesColumn BuildColumn(Column column)
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
        public class Out
        {

            /// <summary>
            /// get or set the results 
            /// <remarks>the dictionary key is actually the key used over cassandra</remarks>
            /// </summary>
            public Dictionary<String, List<GetCommand.Out>> Results 
            {
                get;
                set;
            }

        }
    }
}
