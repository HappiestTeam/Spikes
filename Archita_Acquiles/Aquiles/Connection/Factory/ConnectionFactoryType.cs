using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aquiles.Connection.Factory
{
    /// <summary>
    /// Type of connection used to connect to Cassandra Cluster
    /// </summary>
    public enum ConnectionFactoryType
    {
        /// <summary>
        /// Default connection. It use binary protocol over socket transport
        /// </summary>
        DEFAULT = 0,
        /// <summary>
        /// Buffered connection. It use binary protocol over buffered transport (This is faster than Default)
        /// </summary>
        BUFFERED = 1
    }
}
