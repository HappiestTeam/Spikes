using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apache.Cassandra;
using Aquiles.Exceptions;

namespace Aquiles.Model
{
    /// <summary>
    /// Aquiles structure to contains Cassandra Key Range structure (only support for a Token range get)
    /// <remarks>EndToken might be equal of smaller than the StartToken. If EndToken is equal to StartToken then you will get the full ring</remarks>
    /// </summary>
    public class AquilesTokenRange : IAquilesKeyTokenRange
    {
        /// <summary>
        /// get or set the Start Token (excluded in the command response)
        /// </summary>
        public string StartToken
        {
            get;
            set;
        }

        /// <summary>
        /// get or set the End Token
        /// </summary>
        public string EndToken
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
            keyRange.Start_token = this.StartToken;
            keyRange.End_token = this.EndToken;

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
            if (this.Count <= 0)
            {
                throw new AquilesCommandParameterException("Quantity of keys is required.");
            }

            if (String.IsNullOrEmpty(this.StartToken))
            {
                throw new AquilesCommandParameterException("StartToken must not be null or empty.");
            }

            if (String.IsNullOrEmpty(this.EndToken))
            {
                throw new AquilesCommandParameterException("EndToken must not be null or empty.");
            }
        }

        #endregion
    }
}
