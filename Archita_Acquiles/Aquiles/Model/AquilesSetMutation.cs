﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apache.Cassandra;
using Aquiles.Exceptions;

namespace Aquiles.Model
{
    /// <summary>
    /// Mutation to support set and updage
    /// </summary>
    public class AquilesSetMutation : IAquilesMutation
    {
        /// <summary>
        /// get or set Column information to be inserted
        /// <remarks>Column and SuperColumn are mutual exclusive</remarks>
        /// </summary>
        public AquilesColumn Column
        {
            get;
            set;
        }

        /// <summary>
        /// get or set the SuperColumn information to be inserted
        /// <remarks>Column and SuperColumn are mutual exclusive</remarks>
        /// </summary>
        public AquilesSuperColumn SuperColumn
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
            ColumnOrSuperColumn columnOrSuperColumn = new ColumnOrSuperColumn();
            if (this.Column != null)
            {
                columnOrSuperColumn.Column = this.Column.ToThrift();
            }

            if (this.SuperColumn != null)
            {
                columnOrSuperColumn.Super_column = this.SuperColumn.ToThrift();
            }

            return new Mutation()
                {
                    Column_or_supercolumn = columnOrSuperColumn
                };
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
            this.ValidateMutualExclusion();

            this.ValidateNoSuperColumnOrColumn();
            
            if (this.SuperColumn != null)
            {
                this.SuperColumn.ValidateForSetOperation();
            }
            
            if (this.Column != null)
            {
                this.Column.ValidateForSetOperation();
            }
        }

        private void ValidateNoSuperColumnOrColumn()
        {
            if ((this.SuperColumn == null) && (this.Column == null))
            {
                throw new AquilesCommandParameterException("SuperColumn or Column must be not null.");
            }
        }

        private void ValidateMutualExclusion()
        {
            if ((this.SuperColumn != null) && (this.Column != null))
            {
                throw new AquilesCommandParameterException("SuperColumn and Column are mutually exclusive.");
            }
        }

        #endregion
    }
}
