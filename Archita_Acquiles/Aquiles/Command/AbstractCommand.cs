using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apache.Cassandra;
using Aquiles.Exceptions;

namespace Aquiles.Command
{
    /// <summary>
    /// Abstract Command that adds support for consistency Level over a real command
    /// </summary>
    public abstract class AbstractCommand
    {
        private const AquilesConsistencyLevel DEFAULTCONSISTENCYLEVEL = AquilesConsistencyLevel.QUORUM;

        /// <summary>
        /// Default constructor
        /// </summary>
        protected AbstractCommand()
        {
            this.ConsistencyLevel = DEFAULTCONSISTENCYLEVEL;
        }
        
        /// <summary>
        /// get or set the consistency level required.
        /// <remarks>If you dont know what is this, leave unassigned</remarks>
        /// </summary>
        public AquilesConsistencyLevel ConsistencyLevel
        {
            get;
            set;
        }

        /// <summary>
        /// get the consistencyLevel on Cassandra Thrift structure
        /// </summary>
        protected ConsistencyLevel GetCassandraConsistencyLevel()
        {
            switch (this.ConsistencyLevel)
            {
                case AquilesConsistencyLevel.ALL:
                    return Apache.Cassandra.ConsistencyLevel.ALL;
                case AquilesConsistencyLevel.ANY:
                    return Apache.Cassandra.ConsistencyLevel.ANY;
                case AquilesConsistencyLevel.DCQUORUM:
                    return Apache.Cassandra.ConsistencyLevel.DCQUORUM;
                case AquilesConsistencyLevel.DCQUORUMSYNC:
                    return Apache.Cassandra.ConsistencyLevel.DCQUORUMSYNC;
                case AquilesConsistencyLevel.ONE:
                    return Apache.Cassandra.ConsistencyLevel.ONE;
                case AquilesConsistencyLevel.QUORUM:
                    return Apache.Cassandra.ConsistencyLevel.QUORUM;
                case AquilesConsistencyLevel.ZERO:
                    return Apache.Cassandra.ConsistencyLevel.ZERO;
                default:
                    throw new AquilesException("Unsupported consistency level.");
            }
        }
    }
}
