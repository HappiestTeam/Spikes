using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Aquiles.Helpers.Encoders;

namespace Aquiles.Core.Test
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class EncoderTest
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
        [TestInitialize()]
        public void MyTestInitialize()
        {
        }


        // Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
        }

        #endregion

        [TestMethod()]
        public void LongEncoderHelperTest()
        {
            long value = 8;
            byte[] javaByteLong = ByteEncoderHelper.LongEncoder.ToByteArray(value);
            long newValue = ByteEncoderHelper.LongEncoder.FromByteArray(javaByteLong);
            Assert.AreEqual(value, newValue);
        }

        [TestMethod()]
        public void LittleEndianLongEncoderHelperTest()
        {
            long value = 8;
            byte[] javaByteLong = ByteEncoderHelper.LittleEndianLongEncoder.ToByteArray(value);
            long newValue = ByteEncoderHelper.LittleEndianLongEncoder.FromByteArray(javaByteLong);
            Assert.AreEqual(value, newValue);
        }

        [TestMethod()]
        public void BigEndianLongEncoderHelperTest()
        {
            long value = 8;
            byte[] javaByteLong = ByteEncoderHelper.BigEndianLongEncoder.ToByteArray(value);
            long newValue = ByteEncoderHelper.BigEndianLongEncoder.FromByteArray(javaByteLong);
            Assert.AreEqual(value, newValue);
        }

        [TestMethod()]
        public void AsciiEncoderHelperTest()
        {
            string value = "test value";
            byte[] bytes = ByteEncoderHelper.ASCIIEncoder.ToByteArray(value);
            string value2 = ByteEncoderHelper.ASCIIEncoder.FromByteArray(bytes);
            Assert.AreEqual(value, value2);
        }

        [TestMethod()]
        public void UTF8EncoderHelperTest()
        {
            string value = "test value";
            byte[] bytes = ByteEncoderHelper.UTF8Encoder.ToByteArray(value);
            string value2 = ByteEncoderHelper.UTF8Encoder.FromByteArray(bytes);
            Assert.AreEqual(value, value2);
        }

        [TestMethod()]
        public void GuidEncoderHelperTest()
        {
            Guid value = Guid.NewGuid();
            byte[] bytes = ByteEncoderHelper.GuidEncoder.ToByteArray(value);
            Guid value2 = ByteEncoderHelper.GuidEncoder.FromByteArray(bytes);
            Assert.AreEqual(value, value2);
        }

        [TestMethod()]
        public void UUIDEnconderHelperTest()
        {
            Guid value = Guid.NewGuid();
            byte[] bytes = ByteEncoderHelper.UUIDEnconder.ToByteArray(value);
            Guid value2 = ByteEncoderHelper.UUIDEnconder.FromByteArray(bytes);
            Assert.AreEqual(value, value2);
        }

        /// <summary>
        ///A test for ToByteArray
        ///</summary>
        [TestMethod()]
        public void ToByteArrayTest()
        {
            UUIDEncoderHelper target = new UUIDEncoderHelper();
            Guid value = new Guid("38400000-8cf0-11bd-b23e-10b96e4ef00d");
            Guid expected = new Guid("00004038-f08c-bd11-b23e-10b96e4ef00d");
            byte[] expectedByteArray = expected.ToByteArray();
            byte[] actualByteArray = target.ToByteArray(value);
            string expectedByteArrayString = Stringerize(expectedByteArray);
            string actualByteArrayString = Stringerize(actualByteArray);
            Assert.AreEqual(expectedByteArrayString, actualByteArrayString);
            //            Assert.Inconclusive("Verify the correctness of this test method.");
        }

        private string Stringerize(byte[] expectedByteArray)
        {
            StringBuilder sb = new StringBuilder();
            foreach (byte data in expectedByteArray)
            {
                sb.Append(data);
            }
            return sb.ToString();
        }

        /// <summary>
        ///A test for FromByteArray
        ///</summary>
        [TestMethod()]
        public void FromByteArrayTest()
        {
            UUIDEncoderHelper target = new UUIDEncoderHelper();
            byte[] value = new Guid("00004038-f08c-bd11-b23e-10b96e4ef00d").ToByteArray();
            Guid expected = new Guid("38400000-8cf0-11bd-b23e-10b96e4ef00d");
            Guid actual;
            actual = target.FromByteArray(value);
            Assert.AreEqual(expected.ToString(), actual.ToString());
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }
    }
}
