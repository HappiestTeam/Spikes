using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apache.Cassandra;
using Aquiles.Exceptions;
using Aquiles.Model;
using System.Globalization;

namespace Aquiles.Command
{
    /// <summary>
    /// Abstract class for an AquilesCommand that needs to have a KeySpace
    /// </summary>
    public abstract class AbstractKeySpaceDependantCommand : AbstractCommand
    {
        /// <summary>
        /// get or set the KeySpace
        /// </summary>
        public string KeySpace
        {
            set;
            protected get;
        }

        /// <summary>
        /// Validate the input parameters. 
        /// Throws <see cref="Aquiles.Exceptions.AquilesCommandParameterException"/>  in case there is some malformed or missing input parameters
        /// </summary>
        /// <param name="keyspaces">Dictionary of the keyspaces contained in the cluster that corresponds to the connection</param>
        public virtual void ValidateInput(Dictionary<String,AquilesKeyspace> keyspaces)
        {
            //return !String.IsNullOrEmpty(this.KeySpace);
            if (String.IsNullOrEmpty(this.KeySpace))
            {
                throw new AquilesCommandParameterException("KeySpace must be not null or empty.");
            }
            if (!keyspaces.ContainsKey(this.KeySpace))
            {
                throw new AquilesCommandParameterException(String.Format(CultureInfo.CurrentCulture, "KeySpace '{0}' does not exist on destination cluster.", this.KeySpace));
            }
        }
    }
}
