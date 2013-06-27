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
    /// Summary description for DeleteCommandValidationç
    /// </summary>
    [TestClass]
    public class DeleteCommandValidation
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

            DeleteCommand cmd = new DeleteCommand();
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
            DeleteCommand cmd = new DeleteCommand();
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
            DeleteCommand cmd = new DeleteCommand();
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
            DeleteCommand cmd = new DeleteCommand();
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
            DeleteCommand cmd = new DeleteCommand();
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
            DeleteCommand cmd = new DeleteCommand();
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
            DeleteCommand cmd = new DeleteCommand();
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
        public void OnlyColumn()
        {
            DeleteCommand cmd = new DeleteCommand();
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
            DeleteCommand cmd = new DeleteCommand();
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
            DeleteCommand cmd = new DeleteCommand();
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
            DeleteCommand cmd = new DeleteCommand();
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
            DeleteCommand cmd = new DeleteCommand();
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
        public void NotExistingColumnFamily()
        {

            DeleteCommand cmd = new DeleteCommand();
            cmd.KeySpace = "Keyspace1";
            cmd.ColumnFamily = "Standard12";
            cmd.Key = "test";
            cmd.Column = new Aquiles.Model.AquilesColumn() { ColumnName = "zarasa" };
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

            DeleteCommand cmd = new DeleteCommand();
            cmd.KeySpace = "Keyspace12";
            cmd.ColumnFamily = "Standard1";
            cmd.Key = "test";
            cmd.Column = new Aquiles.Model.AquilesColumn() { ColumnName = "zarasa" };
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
