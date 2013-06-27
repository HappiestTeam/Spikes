using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apache.Cassandra;
using Aquiles.Exceptions;
using Aquiles.Model;
using System.Globalization;

namespace Aquiles.Command
{
    /// <summary>
    /// Abstract class for an AquilesCommand that needs to have a Keyspace and a ColumnFamily
    /// </summary>
    public abstract class AbstractKeySpaceColumnFamilyDependantCommand : AbstractKeySpaceDependantCommand
    {
        /// <summary>
        /// get or set the columnFamily
        /// </summary>
        public string ColumnFamily
        {
            set;
            get;
        }

        /// <summary>
        /// Build Cassandra Thrift ColumnPath
        /// </summary>
        /// <param name="supercolumn">supercolumn name</param>
        /// <param name="column">column name</param>
        /// <returns>Cassandra Thrift ColumnPath</returns>
        protected ColumnPath BuildColumnPath(string supercolumn, string column)
        {
            return this.BuildColumnPath(this.ColumnFamily, supercolumn, column);
        }

        /// <summary>
        /// Build Cassandra Thrift ColumnPath
        /// </summary>
        /// <param name="columnFamily">columnfamily</param>
        /// <param name="supercolumn">supercolumn name</param>
        /// <param name="column">column name</param>
        /// <returns>Cassandra Thrift ColumnPath</returns>
        protected ColumnPath BuildColumnPath(string columnFamily, string supercolumn, string column)
        {
            ColumnPath columnPath = null;
            bool isSuperColumnMissing = String.IsNullOrEmpty(supercolumn);
            bool isColumnMissing = String.IsNullOrEmpty(column);

            columnPath = new ColumnPath();
            columnPath.Column_family = columnFamily;
            if (!isSuperColumnMissing)
            {
                columnPath.Super_column = ByteEncoderHelper.ToByteArray(supercolumn);
            }
            if (!isColumnMissing)
            {
                columnPath.Column = ByteEncoderHelper.ToByteArray(column);
            }

            return columnPath;
        }

        /// <summary>
        /// Validate the input parameters. 
        /// Throws <see cref="Aquiles.Exceptions.AquilesCommandParameterException"/>  in case there is some malformed or missing input parameters
        /// </summary>
        /// <param name="keyspaces">Dictionary of the keyspaces contained in the cluster that corresponds to the connection</param>
        public new virtual void ValidateInput(Dictionary<String,AquilesKeyspace> keyspaces)
        {
            base.ValidateInput(keyspaces);
            this.ValidateColumnFamilyNotNullOrEmpty();
            this.ValidateColumnFamilyExistance(keyspaces);

        }

        private void ValidateColumnFamilyExistance(Dictionary<String, AquilesKeyspace> keyspaces)
        {
            AquilesKeyspace destinationKeySpace = keyspaces[this.KeySpace];
            if (!destinationKeySpace.ColumnFamilies.ContainsKey(this.ColumnFamily))
            {
                throw new AquilesCommandParameterException(String.Format(CultureInfo.CurrentCulture, "ColumnFamily '{0}' does not exist in KeySpace '{1}' on destination cluster.", this.ColumnFamily, this.KeySpace));
            }
        }

        private void ValidateColumnFamilyNotNullOrEmpty()
        {
            if (String.IsNullOrEmpty(this.ColumnFamily))
            {
                throw new AquilesCommandParameterException("ColumnFamily must be not null or empty.");
            }
        }

        /// <summary>
        /// Biuld Thrift ColumnParent structure using ColumnFamily and SuperColumn information
        /// </summary>
        /// <param name="superColumn">name for the supercolumn (null in case there is not one)</param>
        /// <returns>Thrift ColumnParent</returns>
        protected ColumnParent BuildColumnParent(string superColumn)
        {
            ColumnParent columnParent = new ColumnParent();

            bool isSuperColumnMissing = String.IsNullOrEmpty(superColumn);

            columnParent.Column_family = this.ColumnFamily;
            if (!isSuperColumnMissing)
            {
                columnParent.Super_column = ByteEncoderHelper.ToByteArray(superColumn);
            }
            return columnParent;
        }
    }
}
