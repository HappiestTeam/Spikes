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
    /// Summary description for GetCommandValidation
    /// </summary>
    [TestClass]
    public class GetCommandValidation
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

            GetCommand cmd = new GetCommand();
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
        public void OnlyKeyspace()
        {

            GetCommand cmd = new GetCommand();
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
        public void OnlyKeyspaceColumnFamily()
        {

            GetCommand cmd = new GetCommand();
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
        public void OnlyKeyspaceKey()
        {

            GetCommand cmd = new GetCommand();
            cmd.KeySpace = "Keyspace1";
            cmd.Key = "test";
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

            GetCommand cmd = new GetCommand();
            cmd.ColumnFamily = "Standard1";
            cmd.Key = "test";
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

            GetCommand cmd = new GetCommand();
            cmd.KeySpace = "Keyspace12";
            cmd.ColumnFamily = "Standard1";
            cmd.Key = "test";
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

            GetCommand cmd = new GetCommand();
            cmd.KeySpace = "Keyspace1";
            cmd.ColumnFamily = "Standard12";
            cmd.Key = "test";
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

            GetCommand cmd = new GetCommand();
            cmd.Key = "test";
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
