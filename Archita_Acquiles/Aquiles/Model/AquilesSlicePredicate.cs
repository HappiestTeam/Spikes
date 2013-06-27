using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apache.Cassandra;
using Aquiles.Exceptions;

namespace Aquiles.Model
{
    /// <summary>
    ///  Aquiles structure to contains Cassandra Slice Predicate structure
    /// </summary>
    public class AquilesSlicePredicate : IAquilesObject<SlicePredicate>
    {
        /// <summary>
        /// get or set the List of Column Names
        /// <remarks>Columns anr SliceRange are are mutually exclusive</remarks>
        /// </summary>
        public List<String> Columns
        {
            get;
            set;
        }

        /// <summary>
        /// get or set the SliceRange
        /// <remarks>Columns anr SliceRange are are mutually exclusive</remarks>
        /// </summary>
        public AquilesSliceRange SliceRange
        {
            get;
            set;
        }

        #region IAquilesObject<SlicePredicate> Members
        /// <summary>
        /// Convert this object to its equivalent Thrift object
        /// </summary>
        /// <returns>a Thrift Object</returns>
        public SlicePredicate ToThrift()
        {
            SlicePredicate predicate = new SlicePredicate();
            this.BuildThriftColumns(predicate);
            this.BuildThriftSliceRange(predicate);
            return predicate;
        }

        private void BuildThriftSliceRange(SlicePredicate predicate)
        {
            if (this.SliceRange != null)
            {
                predicate.Slice_range = this.SliceRange.ToThrift();
            }
        }

        private void BuildThriftColumns(SlicePredicate predicate)
        {
            if (this.Columns != null)
            {
                predicate.Column_names = new List<byte[]>(this.Columns.Count);
                foreach (string column in this.Columns)
                {
                    predicate.Column_names.Add(ByteEncoderHelper.ToByteArray(column));
                }
            }
        }
        /// <summary>
        /// Validate the object data to assure consistency when used as input parameter when used in an insert Operation
        /// <remarks>Throw <see cref="Aquiles.Exceptions.AquilesCommandParameterException"/> in case there is something wrong</remarks>
        /// </summary>
        public void ValidateForInsertOperation()
        {
            this.Validate();
            if (this.SliceRange != null)
            {
                this.SliceRange.ValidateForInsertOperation();
            }
        }

        /// <summary>
        /// Validate the object data to assure consistency when used as input parameter when used in an deletation Operation
        /// <remarks>Throw <see cref="Aquiles.Exceptions.AquilesCommandParameterException"/> in case there is something wrong</remarks>
        /// </summary>
        public void ValidateForDeletationOperation()
        {
            this.ValidateForDeletationOperation();
        }

        /// <summary>
        /// Validate the object data to assure consistency when used as input parameter when used in an set / update Operation
        /// <remarks>Throw <see cref="Aquiles.Exceptions.AquilesCommandParameterException"/> in case there is something wrong</remarks>
        /// </summary>
        public void ValidateForSetOperation()
        {
            this.Validate();
            if (this.SliceRange != null)
            {
                this.SliceRange.ValidateForSetOperation();
            }
        }

        /// <summary>
        /// Validate the object data to assure consistency when used as input parameter when used in an set / update Operation
        /// <remarks>Throw <see cref="Aquiles.Exceptions.AquilesCommandParameterException"/> in case there is something wrong</remarks>
        /// </summary>
        public void ValidateForQueryOperation()
        {
            this.Validate();
            if (this.SliceRange != null)
            {
                this.SliceRange.ValidateForQueryOperation();
            }
        }

        private void Validate()
        {
            this.ValidateMutualExclusion();

            this.ValidateNoColumnsNorSliceRange();

            if (this.Columns != null) 
            {
                this.ValidateColumns();
            }
        }

        private void ValidateNoColumnsNorSliceRange()
        {
            if ((this.Columns == null) && (this.SliceRange == null))
            {
                throw new AquilesCommandParameterException("Columns or SliceRange information must be present.");
            }
        }

        private void ValidateMutualExclusion()
        {
            if ((this.Columns != null) && (this.SliceRange != null))
            {
                throw new AquilesCommandParameterException("Columns and SliceRange are mutually exclusive.");
            }
        }

        private void ValidateColumns()
        {
            this.ValidateColumnQuantity();
            foreach (string column in this.Columns)
            {
                ValidateColumnNotNullOrEmpty(column);
            }
        }

        private static void ValidateColumnNotNullOrEmpty(string column)
        {
            if (String.IsNullOrEmpty(column))
            {
                throw new AquilesCommandParameterException("Empty ColumnName is not supported.");
            }
        }

        private void ValidateColumnQuantity()
        {
            if (this.Columns.Count == 0)
            {
                throw new AquilesCommandParameterException("No columns especified.");
            }
        }

        #endregion
    }
}
