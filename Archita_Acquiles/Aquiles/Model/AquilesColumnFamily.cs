using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aquiles.Model
{
    /// <summary>
    /// Aquiles structure to hold KeySpace's columnFamily internal information
    /// </summary>
    public class AquilesColumnFamily
    {
        /// <summary>
        /// get or set Name
        /// </summary>
        public string Name
        {
            get;
            set;
        }

        /// <summary>
        /// get or set the ColumnFamilyType
        /// </summary>
        public AquilesColumnFamilyType Type
        {
            get;
            set;
        }
    }
}
