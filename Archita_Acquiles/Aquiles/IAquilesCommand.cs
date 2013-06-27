using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Apache.Cassandra;
using Aquiles.Model;

namespace Aquiles
{
    /// <summary>
    /// Interface for any AquilesCommand with minimum methods to operate with.
    /// </summary>
    public interface IAquilesCommand
    {
        /// <summary>
        /// Execute the command over the opened thrift client
        /// </summary>
        /// <param name="cassandraClient">an opened thrift Client</param>
        void Execute(Cassandra.Client cassandraClient);

        /// <summary>
        /// Validate the input parameters. 
        /// Throws <see cref="Aquiles.Exceptions.AquilesCommandParameterException"/>  in case there is some malformed or missing input parameters
        /// </summary>
        /// <param name="keyspaces">Dictionary of the keyspaces contained in the cluster that corresponds to the connection</param>
        void ValidateInput(Dictionary<String,AquilesKeyspace> keyspaces);
    }
}
