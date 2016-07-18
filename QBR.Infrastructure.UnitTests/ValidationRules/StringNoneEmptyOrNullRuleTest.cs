using QBR.Infrastructure.ValidationRules;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.Windows.Controls;

namespace QBR.Infrastructure.UnitTests
{
    
    
    /// <summary>
    ///This is a test class for StringNoneEmptyOrNullRuleTest and is intended
    ///to contain all StringNoneEmptyOrNullRuleTest Unit Tests
    ///</summary>
    [TestClass()]
    public class StringNoneEmptyOrNullRuleTest
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
        ///A test for Validate
        ///</summary>
        [TestMethod()]
        public void ValidateTest_ValidString()
        {
            //Setup
            var target = new StringNotEmptyOrNullRule();
            var expected = ValidationResult.ValidResult;

            //Exercise
            var actual = target.Validate("TestValidString", null);

            //Verify
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Validate
        ///</summary>
        [TestMethod()]
        public void ValidateTest_EmptyString()
        {
            //Setup
            var target = new StringNotEmptyOrNullRule();
            var expected = new ValidationResult(false, "Field cannot be empty");

            //Exercise
            var actual = target.Validate(String.Empty, null);

            //Verify
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Validate
        ///</summary>
        [TestMethod()]
        public void ValidateTest_NullObject()
        {
            //Setup
            var target = new StringNotEmptyOrNullRule();
            var expected = new ValidationResult(false, "Field cannot be empty");

            //Exercise
            var actual = target.Validate(null, null);

            //Verify
            Assert.AreEqual(expected, actual);
        }
    }
}
