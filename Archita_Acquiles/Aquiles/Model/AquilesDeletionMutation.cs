using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apache.Cassandra;
using Aquiles.Exceptions;

namespace Aquiles.Model
{
    /// <summary>
    /// Mutation to support elimination
    /// </summary>
    public class AquilesDeletionMutation : IAquilesMutation
    {
        /// <summary>
        /// get or set SuperColumn Name
        /// </summary>
        public string SuperColumn
        {
            get;
            set;
        }

        /// <summary>
        /// get or set SuperColumn Timestamp
        /// </summary>
        public long Timestamp
        {
            get;
            set;
        }

        /// <summary>
        /// get or set the predicate to match for the action
        /// </summary>
        public AquilesSlicePredicate Predicate
        {
            get;
            set;
        }

        #region IAquilesObject<Mutation> Members
        
        /// <summary>
        /// Convert this object to its equivalent Thrift object
        /// </summary>
        /// <returns>a Thrift Object</returns>
        public Mutation ToThrift()
        {
            Deletion deletion = new Deletion();
            deletion.Predicate = this.Predicate.ToThrift();
            deletion.Super_column = ByteEncoderHelper.ToByteArray(this.SuperColumn);
            deletion.Timestamp = this.Timestamp;

            return new Mutation() { Deletion = deletion };
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

        /// <summary>
        /// Validate the object data to assure consistency when used as input parameter.
        /// Note: Mutations are exclusive for 1 operation, so there is no need to validate for a type of operation
        /// <remarks>Throw <see cref="Aquiles.Exceptions.AquilesCommandParameterException"/> in case there is something wrong</remarks>
        /// </summary>
        public void Validate()
        {
            this.ValidateNullOrEmptySuperColumn();
            this.ValidateTimestamp();
            this.ValidateNullPredicate();
            this.Predicate.ValidateForDeletationOperation();
        }

        private void ValidateNullPredicate()
        {
            if (this.Predicate == null)
            {
                throw new AquilesCommandParameterException("A predicate must exist.");
            }
        }

        private void ValidateTimestamp()
        {
            if (this.Timestamp <= 0)
            {
                throw new AquilesCommandParameterException("Timestamp must be greater than 0.");
            }
        }

        private void ValidateNullOrEmptySuperColumn()
        {
            if (String.IsNullOrEmpty(this.SuperColumn))
            {
                throw new AquilesCommandParameterException("SuperColumn cannot null or empty.");
            }
        }

        #endregion
    }
}
