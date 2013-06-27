using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apache.Cassandra;
using Aquiles.Exceptions;
using System.Globalization;

namespace Aquiles.Model
{
    /// <summary>
    /// Aquiles structure to contains Cassandra Column structure
    /// </summary>
    public class AquilesColumn : IAquilesObject<Column>
    {
        /// <summary>
        /// Cassandra column name
        /// </summary>
        public string ColumnName
        {
            get;
            set;
        }

        /// <summary>
        /// Cassandra column value
        /// </summary>
        public string Value
        {
            get;
            set;
        }

        /// <summary>
        /// Cassandra Column TimeStamp (must match unix timestamp)
        /// <remarks>Don't mess with this unless you know what you are doing</remarks>
        /// </summary>
        public long? Timestamp
        {
            get;
            set;
        }

        /// <summary>
        /// Empty Constructor
        /// </summary>
        public AquilesColumn()
        {
        }

        #region IAquilesObject<Column> Members
        /// <summary>
        /// Convert this structure into a valid Cassandra Thrift column
        /// </summary>
        /// <returns>a cassandra thrift column</returns>
        public Column ToThrift()
        {
            Column cassandraColumn = new Column();
            cassandraColumn.Name = ByteEncoderHelper.ToByteArray(this.ColumnName);
            cassandraColumn.Value = ByteEncoderHelper.ToByteArray(this.Value);
            this.AssignTimestamp(cassandraColumn);

            return cassandraColumn;
        }

        private void AssignTimestamp(Column cassandraColumn)
        {
            if (this.Timestamp == null)
            {
                cassandraColumn.Timestamp = UnixHelper.UnixTimestamp; 
            }
            else
            {
                cassandraColumn.Timestamp = this.Timestamp.Value;
            }
        }

        /// <summary>
        /// Validate the object data to assure consistency when used as input parameter when used in an insert Operation
        /// <remarks>Throw <see cref="Aquiles.Exceptions.AquilesCommandParameterException"/> in case there is something wrong</remarks>
        /// </summary>
        public void ValidateForInsertOperation()
        {
            this.ValidateNullOrEmptyColumnName();

            this.ValidateNullValue();
        }

        /// <summary>
        /// Validate the object data to assure consistency when used as input parameter when used in an deletation Operation
        /// <remarks>Throw <see cref="Aquiles.Exceptions.AquilesCommandParameterException"/> in case there is something wrong</remarks>
        /// </summary>
        public void ValidateForDeletationOperation()
        {
            this.ValidateNullOrEmptyColumnName();
        }

        /// <summary>
        /// Validate the object data to assure consistency when used as input parameter when used in an set / update Operation
        /// <remarks>Throw <see cref="Aquiles.Exceptions.AquilesCommandParameterException"/> in case there is something wrong</remarks>
        /// </summary>
        public void ValidateForSetOperation()
        {
            this.ValidateNullOrEmptyColumnName();
        }

        /// <summary>
        /// Validate the object data to assure consistency when used as input parameter when used in a Query Operation
        /// <remarks>Throw <see cref="Aquiles.Exceptions.AquilesCommandParameterException"/> in case there is something wrong</remarks>
        /// </summary>
        public void ValidateForQueryOperation()
        {
            this.ValidateNullOrEmptyColumnName();
        }

        private void ValidateNullValue()
        {
            if (this.Value == null)
            {
                throw new AquilesCommandParameterException("Value cannot be null.");
            }
        }

        private void ValidateNullOrEmptyColumnName()
        {
            if (String.IsNullOrEmpty(this.ColumnName))
            {
                throw new AquilesCommandParameterException("ColumnName cannot be null or empty.");
            }
        }
        #endregion

        /// <summary>
        /// overriding ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "Name: '{0}', Timestamp: '{1}', Value: '{2}'",
                this.ColumnName,
                this.Timestamp,
                this.Value);
        }
    }
}
