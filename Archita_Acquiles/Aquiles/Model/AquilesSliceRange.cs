using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apache.Cassandra;
using Aquiles.Exceptions;

namespace Aquiles.Model
{
    /// <summary>
    /// Aquiles structure to contains Cassandra Slice Range structure
    /// </summary>
    public class AquilesSliceRange : IAquilesObject<SliceRange>
    {
        /// <summary>
        /// get or set how many columns to return.
        /// </summary>
        public int Count
        {
            get;
            set;
        }

        /// <summary>
        /// get or set the column name to start the slice with
        /// </summary>
        public string StartColumn
        {
            get;
            set;
        }

        /// <summary>
        /// get or set the column name to finish the slice with
        /// </summary>
        public string FinishColumn
        {
            get;
            set;
        }

        /// <summary>
        /// get or set if the order of the result should be reversed
        /// </summary>
        public bool Reversed
        {
            get;
            set;
        }

        #region IAquilesObject<SliceRange> Members
        /// <summary>
        /// Convert this structure into a valid Cassandra Thrift SuperColumn
        /// </summary>
        /// <returns>a cassandra thrift SuperColumn</returns>
        public SliceRange ToThrift()
        {
            SliceRange sliceRange = new SliceRange();
            sliceRange.Count = this.Count;
            this.AssignStartColumn(sliceRange);
            this.AssignFinishColumn(sliceRange);
            sliceRange.Reversed = this.Reversed;

            return sliceRange;
        }

        private void AssignFinishColumn(SliceRange sliceRange)
        {
            if (String.IsNullOrEmpty(this.FinishColumn))
            {
                sliceRange.Finish = ByteEncoderHelper.ToByteArray(String.Empty);
            }
            else
            {
                sliceRange.Finish = ByteEncoderHelper.ToByteArray(this.FinishColumn);
            }
        }

        private void AssignStartColumn(SliceRange sliceRange)
        {
            if (String.IsNullOrEmpty(this.StartColumn))
            {
                sliceRange.Start = ByteEncoderHelper.ToByteArray(String.Empty);
            }
            else
            {
                sliceRange.Start = ByteEncoderHelper.ToByteArray(this.StartColumn);
            }
        }
        /// <summary>
        /// Validate the object data to assure consistency when used as input parameter when used in an insert Operation
        /// <remarks>Throw <see cref="Aquiles.Exceptions.AquilesCommandParameterException"/> in case there is something wrong</remarks>
        /// </summary>
        public void ValidateForInsertOperation()
        {
            this.Validate();
        }

        /// <summary>
        /// Validate the object data to assure consistency when used as input parameter when used in an deletation Operation
        /// <remarks>Throw <see cref="Aquiles.Exceptions.AquilesCommandParameterException"/> in case there is something wrong</remarks>
        /// </summary>
        public void ValidateForDeletationOperation()
        {
            this.Validate();
        }

        /// <summary>
        /// Validate the object data to assure consistency when used as input parameter when used in an set / update Operation
        /// <remarks>Throw <see cref="Aquiles.Exceptions.AquilesCommandParameterException"/> in case there is something wrong</remarks>
        /// </summary>
        public void ValidateForSetOperation()
        {
            this.Validate();
        }

        /// <summary>
        /// Validate the object data to assure consistency when used as input parameter when used in an set / update Operation
        /// <remarks>Throw <see cref="Aquiles.Exceptions.AquilesCommandParameterException"/> in case there is something wrong</remarks>
        /// </summary>
        public void ValidateForQueryOperation()
        {
            this.Validate();
        }

        private void Validate()
        {
            if (this.Count <= 0)
            {
                throw new AquilesCommandParameterException("Count must be greater than 0.");
            }
        }

        #endregion
    }
}
