using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aquiles.Model;
using Apache.Cassandra;
using Aquiles.Exceptions;
using Aquiles.Logging;
using System.Globalization;

namespace Aquiles.Command
{
    /// <summary>
    /// Command to batch actions over a KeySpace of a Cluster.
    /// Posible actions are:
    ///     - insert
    ///     - delete
    /// </summary>
    public class BatchMutateCommand : AbstractKeySpaceDependantCommand, IAquilesCommand
    {
        private LoggerHelper logger;

        /// <summary>
        /// get or set Mutation actions to be applied
        /// </summary>
        public Dictionary<string, Dictionary<string, List<IAquilesMutation>>> Mutations
        {
            get;
            set;
        }

        /// <summary>
        /// ctor
        /// </summary>
        public BatchMutateCommand()
            : base()
        {
            this.logger = LoggerHelper.CreateLogger(this.GetType());
        }

        #region IAquilesCommand Members
        /// <summary>
        /// Executes a "remove" over the connection. No return values
        /// </summary>
        /// <param name="cassandraClient">opened Thrift client</param>
        public void Execute(Cassandra.Client cassandraClient)
        {
            Dictionary<string, Dictionary<string, List<Mutation>>> mutation_map = this.translate(this.Mutations);
            cassandraClient.batch_mutate(this.KeySpace, mutation_map, this.GetCassandraConsistencyLevel());
        }

        /// <summary>
        /// Validate the input parameters. 
        /// Throws <see cref="Aquiles.Exceptions.AquilesCommandParameterException"/>  in case there is some malformed or missing input parameters
        /// </summary>
        /// <param name="keyspaces">Dictionary of the keyspaces contained in the cluster that corresponds to the connection</param>
        public override void ValidateInput(Dictionary<string, AquilesKeyspace> keyspaces)
        {
            base.ValidateInput(keyspaces);
            // i dont validate KeySpace here, since base class should do it.
            AquilesKeyspace workingKeySpace = keyspaces[this.KeySpace];

            if (this.Mutations != null)
            {
                string key;
                string columnFamily;
                List<IAquilesMutation> mutations;
                Dictionary<string, List<IAquilesMutation>> mutationsOverAKey;
                Dictionary<string, List<IAquilesMutation>>.Enumerator mutationsOverAKeyEnumerator;
                Dictionary<string, Dictionary<string, List<IAquilesMutation>>>.Enumerator keysEnumerator = this.Mutations.GetEnumerator();
                while (keysEnumerator.MoveNext())
                {
                    key = keysEnumerator.Current.Key;
                    if (!String.IsNullOrEmpty(key))
                    {
                        mutationsOverAKey = keysEnumerator.Current.Value;
                        mutationsOverAKeyEnumerator = mutationsOverAKey.GetEnumerator();
                        while (mutationsOverAKeyEnumerator.MoveNext())
                        {
                            columnFamily = mutationsOverAKeyEnumerator.Current.Key;
                            if (!String.IsNullOrEmpty(columnFamily))
                            {
                                if (workingKeySpace.ColumnFamilies.ContainsKey(columnFamily))
                                {
                                    mutations = mutationsOverAKeyEnumerator.Current.Value;
                                    if ((mutations == null) || (mutations != null && mutations.Count == 0))
                                    {
                                        throw new AquilesCommandParameterException(String.Format(CultureInfo.CurrentCulture, "No mutations found for ColumnFamily '{0}' over Key '{1}'.", columnFamily, key));
                                    }
                                    else
                                    {
                                        foreach (IAquilesMutation mutation in mutations)
                                        {
                                            mutation.Validate();
                                        }
                                    }
                                }
                                else
                                {
                                    throw new AquilesCommandParameterException(String.Format(CultureInfo.CurrentCulture, "ColumnFamily '{0}' does not correspond to KeySpace '{1}'.", columnFamily, this.KeySpace));
                                }
                            }
                            else
                            {
                                throw new AquilesCommandParameterException("ColumnFamily cannot be null or empty.");
                            }
                        }
                    }
                    else
                    {
                        throw new AquilesCommandParameterException("Key cannot be null or empty.");
                    }
                }
            }
            else
            {
                throw new AquilesCommandParameterException("No mutations found");
            }
        }

        #endregion

        private Dictionary<string, Dictionary<string, List<Mutation>>> translate(Dictionary<string, Dictionary<string, List<IAquilesMutation>>> mutations)
        {
            Dictionary<string, List<IAquilesMutation>>.Enumerator mutationOverKeyEnumerator;
            Dictionary<string, Dictionary<string, List<Mutation>>> cassandraKeyColumnFamilyMutationMap = new Dictionary<string,Dictionary<string,List<Mutation>>>();
            Dictionary<string, List<Mutation>> cassandraColumnFamilyMutationMap = null;
            List<Mutation> mutationList = null;
            Dictionary<string, Dictionary<string, List<IAquilesMutation>>>.Enumerator mutationEnumerator = mutations.GetEnumerator();
            while (mutationEnumerator.MoveNext())
            {
                cassandraColumnFamilyMutationMap = new Dictionary<string, List<Mutation>>();
                mutationOverKeyEnumerator = mutationEnumerator.Current.Value.GetEnumerator();
                while (mutationOverKeyEnumerator.MoveNext())
                {
                    mutationList = new List<Mutation>(mutationOverKeyEnumerator.Current.Value.Count);
                    foreach (IAquilesMutation aquilesMutation in mutationOverKeyEnumerator.Current.Value)
                    {
                        mutationList.Add(aquilesMutation.ToThrift());
                    }
                    cassandraColumnFamilyMutationMap.Add(mutationOverKeyEnumerator.Current.Key, mutationList);
                }
                cassandraKeyColumnFamilyMutationMap.Add(mutationEnumerator.Current.Key, cassandraColumnFamilyMutationMap);
            }

            return cassandraKeyColumnFamilyMutationMap;
        }
    }
}