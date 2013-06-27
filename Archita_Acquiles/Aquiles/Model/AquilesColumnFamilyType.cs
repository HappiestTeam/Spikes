using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Aquiles.Model
{
    /// <summary>
    /// Type of ColumnFamily
    /// </summary>
    public enum AquilesColumnFamilyType
    {
        /// <summary>
        /// only allows Columns as childs
        /// </summary>
        Standard = 0,
        /// <summary>
        /// only allows SuperColumns as childs
        /// </summary>
        Super = 1
    }
}
