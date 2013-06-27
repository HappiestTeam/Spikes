using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aquiles.Model
{
    /// <summary>
    /// Aquiles representation of a Keyspace
    /// </summary>
    public class AquilesKeyspace
    {
        /// <summary>
        /// get or (internal) set Name
        /// </summary>
        public string Name
        {
            get;
            internal set;
        }

        /// <summary>
        /// get or (internal) set Dictionary of ColumnFamilies where key is the name of the ColumnFamily
        /// </summary>
        public Dictionary<string, AquilesColumnFamily> ColumnFamilies
        {
            get;
            internal set;
        }
    }
}
