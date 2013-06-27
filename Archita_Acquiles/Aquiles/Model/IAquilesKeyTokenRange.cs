using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apache.Cassandra;

namespace Aquiles.Model
{
    /// <summary>
    /// Aquiles structure interface to contains Cassandra Key Range structure
    /// <remarks>Interface for <see cref="Aquiles.Model.AquilesKeyRange"/> and <see cref="Aquiles.Model.AquilesTokenRange"/></remarks>
    /// </summary>
    public interface IAquilesKeyTokenRange : IAquilesObject<KeyRange>
    {
        /// <summary>
        /// get or set how many keys to permit in the KeyRange
        /// </summary>
        int Count
        {
            get;
            set;
        }
    }
}
