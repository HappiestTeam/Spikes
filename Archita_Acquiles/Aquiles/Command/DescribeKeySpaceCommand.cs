using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Aquiles.Model;
using Aquiles.Exceptions;

namespace Aquiles.Command
{
    /// <summary>
    /// Command to retrieve KeySpace structure from cluster.
    /// </summary>
    public class DescribeKeySpaceCommand : AbstractKeySpaceDependantCommand, IAquilesCommand
    {
        private const string COLUMN_TYPE = "Type";

        /// <summary>
        /// get Dictionary of ColumnFamilies where key is the name of the ColumnFamily
        /// </summary>
        public Dictionary<string, AquilesColumnFamily> ColumnFamilies
        {
            get;
            private set;
        }

        #region IAquilesCommand Members
        /// <summary>
        /// Executes a "describe_keyspace" over the connection.
        /// 
        /// Note: This command is not yet finished.
        /// </summary>
        /// <param name="cassandraClient">opened Thrift client</param>
        public void Execute(Apache.Cassandra.Cassandra.Client cassandraClient)
        {
            Dictionary<string, Dictionary<string,string>> keySpaceDescription = cassandraClient.describe_keyspace(this.KeySpace);
            if (keySpaceDescription != null)
            {
                AquilesColumnFamily columnFamily = null;
                Dictionary<string, AquilesColumnFamily> columnFamilies = new Dictionary<string, AquilesColumnFamily>();
                Dictionary<string, Dictionary<string, string>>.Enumerator keySpaceEnumerator = keySpaceDescription.GetEnumerator();
                while (keySpaceEnumerator.MoveNext())
                {
                    columnFamily = this.Translate(keySpaceEnumerator.Current.Key, keySpaceEnumerator.Current.Value);
                    columnFamilies.Add(columnFamily.Name, columnFamily);
                }
                this.ColumnFamilies = columnFamilies;
            }
            else
            {
                this.ColumnFamilies = null;
            }
        }

        /// <summary>
        /// Validate the input parameters. 
        /// Throws <see cref="Aquiles.Exceptions.AquilesCommandParameterException"/>  in case there is some malformed or missing input parameters
        /// </summary>
        /// <param name="keyspaces">Dictionary of the keyspaces contained in the cluster that corresponds to the connection</param>
        public override void ValidateInput(Dictionary<String,AquilesKeyspace> keyspaces)
        {
            /* base method checks for the keyspace existence, we are gathering keyspace information, 
               so at this point such validation is not valid right now */
            if (String.IsNullOrEmpty(this.KeySpace))
            {
                throw new AquilesCommandParameterException("KeySpace must be not null or empty.");
            }
        }
        #endregion

        private AquilesColumnFamily Translate(string columnFamilyName, Dictionary<string, string> data)
        {
            string rawType = null;
            AquilesColumnFamilyType type = AquilesColumnFamilyType.Standard;
            if (data.TryGetValue(COLUMN_TYPE, out rawType))
            {
                type = (AquilesColumnFamilyType)Enum.Parse(typeof(AquilesColumnFamilyType), rawType);
            }
            AquilesColumnFamily columnFamily = new AquilesColumnFamily();
            columnFamily.Name = columnFamilyName;
            columnFamily.Type = type;

            return columnFamily;
        }
    }
}
