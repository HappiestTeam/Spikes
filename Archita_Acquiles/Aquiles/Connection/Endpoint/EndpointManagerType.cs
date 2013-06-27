using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aquiles.Connection.Endpoint
{
    /// <summary>
    /// Types of EndpointManagers to use
    /// </summary>
    public enum EndpointManagerType
    {
        /// <summary>
        /// Cycle through the list of endpoints to balance the pool connections
        /// </summary>
        ROUNDROBIN = 0
    }
}
