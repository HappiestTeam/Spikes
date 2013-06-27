using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apache.Cassandra;
using Aquiles.Exceptions;
using Aquiles.Model;

namespace Aquiles.Command
{
    /// <summary>
    /// Abstract class for an AquilesCommand that needs to have a KeySpace, a ColumnFamily and a Key
    /// </summary>
    public abstract class AbstractKeySpaceColumnFamilyKeyDependantCommand : AbstractKeySpaceColumnFamilyDependantCommand
    {
        /// <summary>
        /// get or set the Key
        /// </summary>
        public string Key
        {
            set;
            get;
        }

        /// <summary>
        /// Validate the input parameters. 
        /// Throws <see cref="Aquiles.Exceptions.AquilesCommandParameterException"/>  in case there is some malformed or missing input parameters
        /// </summary>
        /// <param name="keyspaces">Dictionary of the keyspaces contained in the cluster that corresponds to the connection</param>
        public new virtual void ValidateInput(Dictionary<String,AquilesKeyspace> keyspaces)
        {
            base.ValidateInput(keyspaces);
            if (String.IsNullOrEmpty(this.Key))
            {
                throw new AquilesCommandParameterException("Key must be not null or empty.");
            }
        }
    }
}
