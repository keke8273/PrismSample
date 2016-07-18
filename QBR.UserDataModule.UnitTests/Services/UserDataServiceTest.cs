using Microsoft.VisualStudio.TestTools.UnitTesting;
using QBR.Infrastructure.Models.Enums;
using QBR.UserDataModule.Services;

namespace QBR.UserDataModule.UnitTests.Services
{
    
    
    /// <summary>
    ///This is a test class for UserDataServiceTest and is intended
    ///to contain all UserDataServiceTest Unit Tests
    ///</summary>
    [TestClass()]
    public class UserDataServiceTest
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
        ///A test for IsAllDataCollected
        ///</summary>
        [TestMethod()]
        public void IsAllDataCollectedTest_InitialState_ReturnFalse()
        {
            var target = new UserEntryService();

            var actual = target.IsAllDataCollected();

            Assert.AreEqual(false, actual);
        }

        /// <summary>
        ///A test for IsAllDataCollected
        ///</summary>
        [TestMethod()]
        public void IsAllDataCollectedTest_OperatorIDMissing_ReturnFalse()
        {
            var target = new UserEntryService()
            {
                BatchNumber = "44875",
                OutputDirectory = "TestOutputDirectory",
                TestID = 1
            };

            var actual = target.IsAllDataCollected();

            Assert.AreEqual(false, actual);
        }

        /// <summary>
        ///A test for IsAllDataCollected
        ///</summary>
        [TestMethod()]
        public void IsAllDataCollectedTest_TestIDMissing_ReturnFalse()
        {
            var target = new UserEntryService()
            {
                BatchNumber = "44875",
                OutputDirectory = "TestOutputDirectory",
                OperatorID = "TestOperatorID",
            };

            var actual = target.IsAllDataCollected();

            Assert.AreEqual(false, actual);
        }

        /// <summary>
        ///A test for IsAllDataCollected
        ///</summary>
        [TestMethod()]
        public void IsAllDataCollectedTest_OutputDirectoryMissing_ReturnFalse()
        {
            var target = new UserEntryService()
            {
                BatchNumber = "44875",
                OperatorID = "TestOperatorID",
                TestID = 1
            };

            var actual = target.IsAllDataCollected();

            Assert.AreEqual(false, actual);
        }

        /// <summary>
        ///A test for IsAllDataCollected
        ///</summary>
        [TestMethod()]
        public void IsAllDataCollectedTest_BatchNumberMissing_ReturnFalse()
        {
            var target = new UserEntryService()
            {
                OperatorID = "TestOperator",
                OutputDirectory = "TestOutputDirectory",
                TestID = 1
            };

            var actual = target.IsAllDataCollected();

            Assert.AreEqual(false, actual);
        }

        /// <summary>
        ///A test for IsAllDataCollected
        ///</summary>
        [TestMethod()]
        public void IsAllDataCollectedTest_AllCollected_ReturnTrue()
        {
            var target = new UserEntryService()
            {
                OperatorID = "TestOperator",
                BatchNumber = "44875",
                OutputDirectory = "TestOutputDirectory",
                TestID = 1,
                BankID = 1,
                StripType = StripType.Proteus,
                TestTarget = "TestTestTarget",
            };

            var actual = target.IsAllDataCollected();

            Assert.AreEqual(true, actual);
        }
    }
}
