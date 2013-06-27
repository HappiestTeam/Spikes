using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using Aquiles.Exceptions;
//using Aquiles.Exceptions;

namespace Aquiles.Configuration
{
    /// <summary>
    /// Exception thrown when there are invalid or missing values over the configuration
    /// </summary>
    public class AquilesConfigurationException : AquilesException
    {
        /// <summary>
        /// ctor
        /// </summary>
        public AquilesConfigurationException() : base() { }
        /// <summary>
        /// ctor
        /// </summary>
        public AquilesConfigurationException(string message) : base(message) { }
        /// <summary>
        /// ctor
        /// </summary>
        public AquilesConfigurationException(string message, Exception ex) : base(message, ex) { }
    }
}
