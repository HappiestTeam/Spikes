using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aquiles.Core.Test
{
    /// <summary>
    ///This is a test class for GuidGeneratorTest and is intended
    ///to contain all GuidGeneratorTest Unit Tests
    ///</summary>
    [TestClass()]
    public class GuidGeneratorTest
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
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for GenerateTimeBasedGuid
        ///</summary>
        [TestMethod()]
        public void GenerateTimeBasedGuidTest2()
        {
            int cycles = 100;

            HashSet<Guid> generatedGuids = new HashSet<Guid>();
            for (int i = 0; i < cycles; i++)
            {
                Guid guid = Aquiles.Helpers.GuidGenerator.GenerateTimeBasedGuid();
                Assert.IsTrue(generatedGuids.Add(guid));
            }

        }
    }
}
