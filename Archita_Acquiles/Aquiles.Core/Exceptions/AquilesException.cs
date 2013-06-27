using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aquiles.Core.Exceptions
{
    /// <summary>
    /// Exception thrown when something went wrong inside Aquiles
    /// </summary>
    [Serializable]
    public class AquilesException : Exception
    {
        /// <summary>
        /// ctor
        /// </summary>
        public AquilesException() : base() { }
        /// <summary>
        /// ctor
        /// </summary>
        public AquilesException(string message) : base(message) { }
        /// <summary>
        /// ctor
        /// </summary>
        public AquilesException(string message, Exception ex) : base(message, ex) { }
    }
}
