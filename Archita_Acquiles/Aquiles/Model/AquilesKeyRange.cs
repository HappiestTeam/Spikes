using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apache.Cassandra;
using Aquiles.Exceptions;

namespace Aquiles.Model
{
    /// <summary>
    /// Aquiles structure to contains Cassandra Key Range structure (only support for a key range get)
    /// <remarks>StartKey will be included in the response</remarks>
    /// </summary>
    public class AquilesKeyRange : IAquilesKeyTokenRange
    {
        /// <summary>
        /// get or set the Start Key (included in the command response)
        /// </summary>
        public string StartKey
        {
            get;
            set;
        }

        /// <summary>
        /// get or set the Endkey
        /// </summary>
        public string EndKey
        {
            get;
            set;
        }

        #region IAquilesObject<KeyRange> Members

        /// <summary>
        /// get or set how many keys to permit in the KeyRange
        /// </summary>
        public int Count
        {
            get;
            set;
        }
        
        /// <summary>
        /// Convert this structure into a valid Cassandra Thrift KeyRange
        /// </summary>
        /// <returns>a cassandra thrift KeyRange</returns>
        public KeyRange ToThrift()
        {
            KeyRange keyRange = new KeyRange();
            keyRange.Count = this.Count;
            keyRange.Start_key = this.StartKey;
            keyRange.End_key = this.EndKey;

            return keyRange;
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
            this.ValidateCountGreaterThanZero();
            this.ValidateNullOrEmptyStartKey();
            this.ValidateNullOrEmptyEndKey();
        }

        private void ValidateNullOrEmptyEndKey()
        {
            if (String.IsNullOrEmpty(this.EndKey))
            {
                throw new AquilesCommandParameterException("EndKey must not be null or empty.");
            }
        }

        private void ValidateNullOrEmptyStartKey()
        {
            if (String.IsNullOrEmpty(this.StartKey))
            {
                throw new AquilesCommandParameterException("StartKey must not be null or empty.");
            }
        }

        private void ValidateCountGreaterThanZero()
        {
            if (this.Count <= 0)
            {
                throw new AquilesCommandParameterException("Quantity of keys is required.");
            }
        }

        #endregion
    }
}
