using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aquiles.Connection.Pooling
{
    /// <summary>
    /// Type of ConnectionPool
    /// </summary>
    public enum ConnectionPoolType
    {
        /// <summary>
        /// No pool is used. Clients are created based on need, they are disposed as soon as they are returned.
        /// </summary>
        NOPOOL = 0,
        /// <summary>
        /// Warmup enable and size-controlled enabled pool.
        /// </summary>
        SIZECONTROLLEDPOOL = 1

    }
}
