using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aquiles.Exceptions
{
    /// <summary>
    /// Exception thrown when a command input parameters are not valid
    /// </summary>
    [Serializable]
    public class AquilesCommandException : AquilesException
    {
        /// <summary>
        /// ctor
        /// </summary>
        public AquilesCommandException() : base() { }
        /// <summary>
        /// ctor
        /// </summary>
        public AquilesCommandException(string message) : base(message) { }
        /// <summary>
        /// ctor
        /// </summary>
        public AquilesCommandException(string message, Exception ex) : base(message, ex) { }
    }
}
