using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Aquiles.Configuration;
using System.Configuration;
using Aquiles.Command;
using Apache.Cassandra;
using Thrift.Protocol;
using Thrift.Transport;
using Aquiles.Model;
using System.Threading;
using Aquiles.Exceptions;

namespace Aquiles.Test
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class AquilesHelperTest
    {

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion




        [TestMethod]
        public void OpenAndCloseConnection()
        {
            using (IAquilesConnection connection = AquilesHelper.RetrieveConnection("Keyspace1"))
            {
                connection.Open();
                connection.Close();
            }
        }

        [TestMethod]
        public void OpenAndCloseConnectionOnNonExistintCluster()
        {
            try
            {
                IAquilesConnection connection = AquilesHelper.RetrieveConnection("NonExistsCluster");
                Assert.Fail();
            }
            catch (AquilesException)
            {
                //do nothing, this is expected
            }
        }

        //[TestMethod]
        //public void RetrieveKeySpacesWithSeveralConnections()
        //{
        //    RetrieveKeySpacesCommand retrieveKeySpacesCommand = new RetrieveKeySpacesCommand();
        //    using (IAquilesConnection connection = AquilesHelper.RetrieveConnection("Keyspace1"),
        //        connection2 = AquilesHelper.RetrieveConnection("Keyspace1"),
        //        connection3 = AquilesHelper.RetrieveConnection("Keyspace1"),
        //        connection4 = AquilesHelper.RetrieveConnection("Keyspace1"))
        //    {
        //        connection.Open();
        //        connection2.Open();
        //        connection3.Open();
        //        connection4.Open();
        //        connection.Execute(retrieveKeySpacesCommand);
        //        connection2.Execute(retrieveKeySpacesCommand);
        //        connection3.Execute(retrieveKeySpacesCommand);
        //        connection4.Execute(retrieveKeySpacesCommand);
        //        connection.Close();
        //        connection2.Close();
        //        connection3.Close();
        //        connection4.Close();

        //    }

        //    Assert.IsNotNull(retrieveKeySpacesCommand.KeySpaces);
        //}

        //[TestMethod]
        public void ReuseConnectionsOnthePool()
        {
            
            RetrieveKeySpacesCommand retrieveKeySpacesCommand = new RetrieveKeySpacesCommand();
            using (IAquilesConnection connection = AquilesHelper.RetrieveConnection("Keyspace1"))
            {
                connection.Open();
                connection.Execute(retrieveKeySpacesCommand);
                connection.Close();

            }

            using (IAquilesConnection connection = AquilesHelper.RetrieveConnection("Keyspace1"))
            {
                connection.Open();
                connection.Execute(retrieveKeySpacesCommand);
                connection.Close();

            }
        }

        [TestMethod]
        public void TestInsertDeleteAndGetOnDifferentConnections()
        {
            string columnFamily = "Standard1";
            string keyspace = "Keyspace1";
            string key = Guid.NewGuid().ToString();
            string columnName = "testColumn";
            string columnValue = "testValue";

            DoInsertDeleteAndGetOnDifferenteConnections(columnFamily, keyspace, key, columnName, columnValue);
        }

        private static void DoInsertDeleteAndGetOnDifferenteConnections(string columnFamily, string keyspace, string key, string columnName, string columnValue)
        {
            // Insert statement
            InsertCommand insertCommand = new InsertCommand();
            insertCommand.KeySpace = keyspace;
            insertCommand.ColumnFamily = columnFamily;
            insertCommand.Key = key;
            insertCommand.Column = new Aquiles.Model.AquilesColumn()
            {
                ColumnName = columnName,
                Value = columnValue
            };

            using (IAquilesConnection connection = AquilesHelper.RetrieveConnection("Keyspace1"))
            {
                connection.Execute(insertCommand);
            }

            // Get statement

            GetCommand getCommand = new GetCommand();
            getCommand.KeySpace = keyspace;
            getCommand.Key = key;
            getCommand.ColumnFamily = columnFamily;
            getCommand.ColumnName = columnName;

            using (IAquilesConnection connection = AquilesHelper.RetrieveConnection("Keyspace1"))
            {
                connection.Execute(getCommand);
            }

            GetCommand.Out output = getCommand.Output;

            Assert.IsNotNull(output);
            Assert.IsNotNull(output.Column);
            Assert.IsTrue(columnName.CompareTo(output.Column.ColumnName) == 0);
            Assert.IsTrue(columnValue.CompareTo(output.Column.Value) == 0);

            //Delete statement
            DeleteCommand delCommand = new DeleteCommand();
            delCommand.KeySpace = keyspace;
            delCommand.Key = key;
            delCommand.ColumnFamily = columnFamily;
            delCommand.Column = output.Column;
            using (IAquilesConnection connection = AquilesHelper.RetrieveConnection("Keyspace1"))
            {
                connection.Execute(delCommand);
            }

            // get statement to see if it was actually deleted (get is already created, then i am reusing)
            using (IAquilesConnection connection = AquilesHelper.RetrieveConnection("Keyspace1"))
            {
                connection.Execute(getCommand);
            }

            Assert.IsNull(getCommand.Output);
        }

        [TestMethod]
        public void TestInsertDeleteAndGetOnSameConnection()
        {
            string columnFamily = "Standard1";
            string keyspace = "Keyspace1";
            string key = Guid.NewGuid().ToString();
            string columnName = "testColumn";
            string columnValue = "testValue";

            DoTestInsertDeleteAndGetOnSameConnection(columnFamily, keyspace, key, columnName, columnValue);
        }

        private static void DoTestInsertDeleteAndGetOnSameConnection(string columnFamily, string keyspace, string key, string columnName, string columnValue)
        {
            // Insert statement
            InsertCommand insertCommand = new InsertCommand();
            insertCommand.KeySpace = keyspace;
            insertCommand.ColumnFamily = columnFamily;
            insertCommand.Key = key;
            insertCommand.Column = new Aquiles.Model.AquilesColumn()
            {
                ColumnName = columnName,
                Value = columnValue
            };

            // Get statement

            GetCommand getCommand = new GetCommand();
            getCommand.KeySpace = keyspace;
            getCommand.Key = key;
            getCommand.ColumnFamily = columnFamily;
            getCommand.ColumnName = columnName;

            //Delete statement
            DeleteCommand delCommand = new DeleteCommand();
            delCommand.KeySpace = keyspace;
            delCommand.Key = key;
            delCommand.ColumnFamily = columnFamily;

            using (IAquilesConnection connection = AquilesHelper.RetrieveConnection("Keyspace1"))
            {
                connection.Open();

                connection.Execute(insertCommand);
                connection.Execute(getCommand);

                GetCommand.Out output = getCommand.Output;

                Assert.IsNotNull(output);
                Assert.IsNotNull(output.Column);
                Assert.IsTrue(columnName.CompareTo(output.Column.ColumnName) == 0);
                Assert.IsTrue(columnValue.CompareTo(output.Column.Value) == 0);

                delCommand.Column = output.Column;

                connection.Execute(delCommand);

                connection.Execute(getCommand);

                connection.Close();

                Assert.IsNull(getCommand.Output);
            }
        }

        [TestMethod]
        public void TestStressPortHandling()
        {
            for (int i = 0; i < 10; i++)
            {
                this.TestInsertDeleteAndGetOnDifferentConnections();
            }
        }

        //[TestMethod]
        public void TestSleep()
        {
            System.Threading.Thread.Sleep(3600000); //1 min
        }

        [TestMethod]
        public void TestBatchMutation()
        {
            Guid id = Guid.NewGuid();
            BatchMutateCommand cmd = new BatchMutateCommand();
            cmd.KeySpace = "Keyspace1";
            Dictionary<string, Dictionary<string, List<IAquilesMutation>>>  keyMutations = new Dictionary<string, Dictionary<string, List<IAquilesMutation>>>();
            Dictionary<string,List<IAquilesMutation>> columnMutation = new Dictionary<string,List<IAquilesMutation>>();
            List<IAquilesMutation> mutations = new List<IAquilesMutation>();

            AquilesSetMutation insertContextDataMutation = new AquilesSetMutation();
            insertContextDataMutation.SuperColumn = new AquilesSuperColumn()
            {
                Name = "Data",
                Columns = new List<AquilesColumn>()
            };

            insertContextDataMutation.SuperColumn.Columns.Add(this.CreateColumn("Created", DateTime.UtcNow));
            insertContextDataMutation.SuperColumn.Columns.Add(this.CreateColumn("LockDate", DateTime.UtcNow));
            insertContextDataMutation.SuperColumn.Columns.Add(this.CreateColumn("LockID", 0));
            insertContextDataMutation.SuperColumn.Columns.Add(this.CreateColumn("Locked", false));
            insertContextDataMutation.SuperColumn.Columns.Add(this.CreateColumn("Flags", 0));
            
            mutations.Add(this.CreateSessionDataInsert());
            mutations.Add(insertContextDataMutation);

            columnMutation.Add("Super1", mutations);
            string key = id.ToString();
            keyMutations.Add(key, columnMutation);
            cmd.Mutations = keyMutations;

            GetSliceCommand getSliceCmd = new GetSliceCommand()
            {
                KeySpace = "Keyspace1",
                ColumnFamily = "Super1",
                Key = key,
                Predicate = new AquilesSlicePredicate()
                {
                    SliceRange = new AquilesSliceRange()
                    {
                        Count = int.MaxValue,
                        Reversed = true
                    }
                }
            };

            DeleteCommand delCommand = new DeleteCommand()
            {
                KeySpace = "Keyspace1",
                ColumnFamily = "Super1",
                Key = key
            };

            using (IAquilesConnection connection = AquilesHelper.RetrieveConnection("Keyspace1"))
            {
                connection.Open();
                connection.Execute(cmd);
                connection.Execute(getSliceCmd);
                connection.Execute(delCommand);
                connection.Close();
            }
        }

        private AquilesColumn CreateColumn(string name, object value)
        {
            return new AquilesColumn()
                {
                    ColumnName = name,
                    Value = value.ToString()
                };
        }

        private IAquilesMutation CreateSessionDataInsert()
        {
            AquilesSetMutation insertMutation = new AquilesSetMutation();
            insertMutation.SuperColumn = new AquilesSuperColumn()
                {
                    Name = "SessionDataRAW",
                    Columns = new List<AquilesColumn>()
                };

            insertMutation.SuperColumn.Columns.Add(new AquilesColumn() 
                { 
                    ColumnName = "Data", 
                    Value = "DATA"
                });
            
            return insertMutation;
        }
    }
}
