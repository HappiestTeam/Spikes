using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Thrift.Protocol;

namespace Aquiles.Model
{
    /// <summary>
    /// Interface for model object in order to support common methods
    /// </summary>
    /// <typeparam name="T">must derive from Thrift.Protocol.TBase</typeparam>
    public interface IAquilesObject<T> where T : TBase
    {
        /// <summary>
        /// Convert this object to its equivalent Thrift object
        /// </summary>
        /// <returns>a Thrift Object</returns>
        T ToThrift();

        /// <summary>
        /// Validate the object data to assure consistency when used as input parameter when used in an insert Operation
        /// <remarks>Throw <see cref="Aquiles.Exceptions.AquilesCommandParameterException"/> in case there is something wrong</remarks>
        /// </summary>
        void ValidateForInsertOperation();

        /// <summary>
        /// Validate the object data to assure consistency when used as input parameter when used in an deletation Operation
        /// <remarks>Throw <see cref="Aquiles.Exceptions.AquilesCommandParameterException"/> in case there is something wrong</remarks>
        /// </summary>
        void ValidateForDeletationOperation();

        /// <summary>
        /// Validate the object data to assure consistency when used as input parameter when used in an set / update Operation
        /// <remarks>Throw <see cref="Aquiles.Exceptions.AquilesCommandParameterException"/> in case there is something wrong</remarks>
        /// </summary>
        void ValidateForSetOperation();

        /// <summary>
        /// Validate the object data to assure consistency when used as input parameter when used in an query Operation
        /// <remarks>Throw <see cref="Aquiles.Exceptions.AquilesCommandParameterException"/> in case there is something wrong</remarks>
        /// </summary>
        void ValidateForQueryOperation();

    }
}
