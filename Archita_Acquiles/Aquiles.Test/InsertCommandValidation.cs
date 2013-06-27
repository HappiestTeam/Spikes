using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Aquiles.Command;
using Aquiles.Exceptions;

namespace Aquiles.Test
{
    /// <summary>
    /// Summary description for InsertCommandValidationç
    /// </summary>
    [TestClass]
    public class InsertCommandValidation
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
        public void NoInput()
        {

            InsertCommand cmd = new InsertCommand();
            try
            {
                using (IAquilesConnection connection = AquilesHelper.RetrieveConnection("Keyspace1"))
                {
                    connection.Execute(cmd);
                }
                Assert.Fail();
            }
            catch (AquilesCommandParameterException)
            {
                // this is supposed to happen
            }
        }

        [TestMethod]
        public void OnlyKeySpace()
        {
            InsertCommand cmd = new InsertCommand();
            cmd.KeySpace = "Keyspace1";
            try
            {
                using (IAquilesConnection connection = AquilesHelper.RetrieveConnection("Keyspace1"))
                {
                    connection.Execute(cmd);
                }
                Assert.Fail();
            }
            catch (AquilesCommandParameterException)
            {
                // this is supposed to happen
            }
        }

        [TestMethod]
        public void OnlyColmnFamily()
        {
            InsertCommand cmd = new InsertCommand();
            cmd.ColumnFamily = "Standard1";
            try
            {
                using (IAquilesConnection connection = AquilesHelper.RetrieveConnection("Keyspace1"))
                {
                    connection.Execute(cmd);
                }
                Assert.Fail();
            }
            catch (AquilesCommandParameterException)
            {
                // this is supposed to happen
            }
        }

        [TestMethod]
        public void OnlyKeySpaceColmnFamily()
        {
            InsertCommand cmd = new InsertCommand();
            cmd.KeySpace = "Keyspace1";
            cmd.ColumnFamily = "Standard1";
            try
            {
                using (IAquilesConnection connection = AquilesHelper.RetrieveConnection("Keyspace1"))
                {
                    connection.Execute(cmd);
                }
                Assert.Fail();
            }
            catch (AquilesCommandParameterException)
            {
                // this is supposed to happen
            }
        }

        [TestMethod]
        public void OnlyKey()
        {
            InsertCommand cmd = new InsertCommand();
            cmd.Key = "zarasa";
            try
            {
                using (IAquilesConnection connection = AquilesHelper.RetrieveConnection("Keyspace1"))
                {
                    connection.Execute(cmd);
                }
                Assert.Fail();
            }
            catch (AquilesCommandParameterException)
            {
                // this is supposed to happen
            }
        }

        [TestMethod]
        public void OnlyKeySpaceKey()
        {
            InsertCommand cmd = new InsertCommand();
            cmd.Key = "zarasa";
            cmd.KeySpace = "Keyspace1";
            try
            {
                using (IAquilesConnection connection = AquilesHelper.RetrieveConnection("Keyspace1"))
                {
                    connection.Execute(cmd);
                }
                Assert.Fail();
            }
            catch (AquilesCommandParameterException)
            {
                // this is supposed to happen
            }
        }

        [TestMethod]
        public void OnlyColumnFamilyKey()
        {
            InsertCommand cmd = new InsertCommand();
            cmd.Key = "zarasa";
            cmd.ColumnFamily = "Standard1";
            try
            {
                using (IAquilesConnection connection = AquilesHelper.RetrieveConnection("Keyspace1"))
                {
                    connection.Execute(cmd);
                }
                Assert.Fail();
            }
            catch (AquilesCommandParameterException)
            {
                // this is supposed to happen
            }
        }

        [TestMethod]
        public void OnlyKeySpaceColmnFamilyKey()
        {
            InsertCommand cmd = new InsertCommand();
            cmd.KeySpace = "Keyspace1";
            cmd.ColumnFamily = "Standard1";
            cmd.Key = "zarasa";
            try
            {
                using (IAquilesConnection connection = AquilesHelper.RetrieveConnection("Keyspace1"))
                {
                    connection.Execute(cmd);
                }
                Assert.Fail();
            }
            catch (AquilesCommandParameterException)
            {
                // this is supposed to happen
            }
        }

        [TestMethod]
        public void OnlyColumn()
        {
            InsertCommand cmd = new InsertCommand();
            cmd.Column = new Aquiles.Model.AquilesColumn()
            {
                ColumnName = "columnName"
            };
            try
            {
                using (IAquilesConnection connection = AquilesHelper.RetrieveConnection("Keyspace1"))
                {
                    connection.Execute(cmd);
                }
                Assert.Fail();
            }
            catch (AquilesCommandParameterException)
            {
                // this is supposed to happen
            }
        }

        [TestMethod]
        public void OnlyKeySpaceColumn()
        {
            InsertCommand cmd = new InsertCommand();
            cmd.KeySpace = "Keyspace1";
            cmd.Column = new Aquiles.Model.AquilesColumn()
            {
                ColumnName = "columnName"
            };
            try
            {
                using (IAquilesConnection connection = AquilesHelper.RetrieveConnection("Keyspace1"))
                {
                    connection.Execute(cmd);
                }
                Assert.Fail();
            }
            catch (AquilesCommandParameterException)
            {
                // this is supposed to happen
            }
        }

        [TestMethod]
        public void OnlyKeySpaceColumnFamilyColumn()
        {
            InsertCommand cmd = new InsertCommand();
            cmd.KeySpace = "Keyspace1";
            cmd.ColumnFamily = "Standard1";
            cmd.Column = new Aquiles.Model.AquilesColumn()
            {
                ColumnName = "columnName"
            };
            try
            {
                using (IAquilesConnection connection = AquilesHelper.RetrieveConnection("Keyspace1"))
                {
                    connection.Execute(cmd);
                }
                Assert.Fail();
            }
            catch (AquilesCommandParameterException)
            {
                // this is supposed to happen
            }
        }

        [TestMethod]
        public void OnlyKeySpaceColumnFamilyKeyColumn()
        {
            InsertCommand cmd = new InsertCommand();
            cmd.KeySpace = "Keyspace1";
            cmd.ColumnFamily = "Standard1";
            cmd.Key = "zarasa";
            cmd.Column = new Aquiles.Model.AquilesColumn()
            {
                ColumnName = "columnName"
            };
            try
            {
                using (IAquilesConnection connection = AquilesHelper.RetrieveConnection("Keyspace1"))
                {
                    connection.Execute(cmd);
                }
                Assert.Fail();
            }
            catch (AquilesCommandParameterException)
            {
                // this is supposed to happen
            }
        }

        [TestMethod]
        public void OnlyKeySpaceColumnFamilyKeyColumnForcingNullAsColumnValue()
        {
            InsertCommand cmd = new InsertCommand();
            cmd.KeySpace = "Keyspace1";
            cmd.ColumnFamily = "Standard1";
            cmd.Key = "zarasa";
            cmd.Column = new Aquiles.Model.AquilesColumn()
            {
                ColumnName = "columnName",
                Value = null
            };
            try
            {
                using (IAquilesConnection connection = AquilesHelper.RetrieveConnection("Keyspace1"))
                {
                    connection.Execute(cmd);
                }
                Assert.Fail();
            }
            catch (AquilesCommandParameterException)
            {
                // this is supposed to happen
            }
        }

        [TestMethod]
        public void NotExistingColumnFamily()
        {

            InsertCommand cmd = new InsertCommand();
            cmd.KeySpace = "Keyspace1";
            cmd.ColumnFamily = "Standard12";
            cmd.Key = "test";
            cmd.Column = new Aquiles.Model.AquilesColumn() { ColumnName = "zarasa", Value = "zarasa" };
            try
            {
                using (IAquilesConnection connection = AquilesHelper.RetrieveConnection("Keyspace1"))
                {
                    connection.Execute(cmd);
                }
                Assert.Fail();
            }
            catch (AquilesCommandParameterException)
            {
                // this is supposed to happen
            }
        }

        [TestMethod]
        public void NotExistingKeySpace()
        {

            InsertCommand cmd = new InsertCommand();
            cmd.KeySpace = "Keyspace12";
            cmd.ColumnFamily = "Standard1";
            cmd.Key = "test";
            cmd.Column = new Aquiles.Model.AquilesColumn() { ColumnName = "zarasa", Value = "zarasa" };
            try
            {
                using (IAquilesConnection connection = AquilesHelper.RetrieveConnection("Keyspace1"))
                {
                    connection.Execute(cmd);
                }
                Assert.Fail();
            }
            catch (AquilesCommandParameterException)
            {
                // this is supposed to happen
            }
        }
    }
}
