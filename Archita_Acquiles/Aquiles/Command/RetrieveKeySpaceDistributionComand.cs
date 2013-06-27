using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aquiles.Model;
using Apache.Cassandra;

namespace Aquiles.Command
{
    /// <summary>
    /// Command to retrieve the keyspace's token distribution over the nodes from a Cluster (the real name)
    /// </summary>
    public class RetrieveKeySpaceDistributionComand : AbstractKeySpaceDependantCommand, IAquilesCommand
    {
        /// <summary>
        /// Get the TokenRanges information
        /// </summary>
        public HashSet<Out> Output
        {
            get;
            private set;
        }

        #region IAquilesCommand Members

        /// <summary>
        /// Executes a "describe_ring" over the connection, set the Version property with the returned value.
        /// </summary>
        /// <param name="cassandraClient">opened Thrift client</param>
        public void Execute(Apache.Cassandra.Cassandra.Client cassandraClient)
        {
            List<TokenRange> results = cassandraClient.describe_ring(this.KeySpace);
            this.BuildOut(results);
        }

        /// <summary>
        /// Validate the input parameters. 
        /// Throws <see cref="Aquiles.Exceptions.AquilesCommandParameterException"/>  in case there is some malformed or missing input parameters
        /// </summary>
        /// <param name="keyspaces">Dictionary of the keyspaces contained in the cluster that corresponds to the connection</param>
        public override void ValidateInput(Dictionary<string, AquilesKeyspace> keyspaces)
        {
            base.ValidateInput(keyspaces);
        }
        #endregion

        private void BuildOut(List<TokenRange> results)
        {
            if (results != null)
            {
                this.Output = new HashSet<Out>();
                foreach (TokenRange tokenRange in results)
                {
                    this.Output.Add(new Out()
                    {
                        StartToken = tokenRange.Start_token,
                        EndToken = tokenRange.End_token,
                        Endpoints = tokenRange.Endpoints
                    });    
                }
            }
        }

        /// <summary>
        /// Ouput or return class for the RetrieveKeySpaceDistributionCommand
        /// </summary>
        public class Out
        {
            /// <summary>
            /// get or set the Start Token
            /// </summary>
            public string StartToken
            {
                get;
                set;
            }

            /// <summary>
            /// get or set the End Token
            /// </summary>
            public string EndToken
            {
                get;
                set;
            }

            /// <summary>
            /// get or set the list of endpoints within the cluster that have the token information
            /// </summary>
            public List<string> Endpoints
            {
                get;
                set;
            }
        }
    }
}
