using System;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using QBR.Infrastructure.ValidationRules;

namespace QBR.Infrastructure.UnitTests.ValidationRules
{
    
    
    /// <summary>
    ///This is a test class for IntegerRangeRuleTest and is intended
    ///to contain all IntegerRangeRuleTest Unit Tests
    ///</summary>
    [TestClass()]
    public class Int32RangeCheckTest
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
        public void ValidateTest_NullObject()
        {
            //Setup
            var target = new Int32RangeCheck()
            {
                Max = 100,
                Min = 1
            };
            var expected = new ValidationResult(false, "Field cannot be empty");

            //Exercise
            var actual = target.Validate(null, null);

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
            var target = new Int32RangeCheck()
            {
                Max = 100,
                Min = 1
            };
            var expected = new ValidationResult(false, "Field cannot be empty");

            //Exercise
            var actual = target.Validate(string.Empty, null);

            //Verify
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Validate
        ///</summary>
        [TestMethod()]
        public void ValidateTest_InputTooBig()
        {
            //Setup
            var target = new Int32RangeCheck()
            {
                Max = 100,
                Min = 1
            };
            var expected = new ValidationResult(false, "Enter a value between 1 and 100.");

            //Exercise
            var actual = target.Validate("101", null);

            //Verify
            Assert.AreEqual(expected.ErrorContent, actual.ErrorContent);
            Assert.AreEqual(expected.IsValid, actual.IsValid);
        }

        /// <summary>
        ///A test for Validate
        ///</summary>
        [TestMethod()]
        public void ValidateTest_InputTooSmall()
        {
            //Setup
            var target = new Int32RangeCheck()
            {
                Max = 100,
                Min = 1
            };
            var expected = new ValidationResult(false, "Enter a value between 1 and 100.");

            //Exercise
            var actual = target.Validate("0", null);

            //Verify
            Assert.AreEqual(expected.ErrorContent, actual.ErrorContent);
            Assert.AreEqual(expected.IsValid, actual.IsValid);
        }

        /// <summary>
        ///A test for Validate
        ///</summary>
        [TestMethod()]
        public void ValidateTest_IllegalCharacters()
        {
            //Setup
            var target = new Int32RangeCheck()
            {
                Max = 100,
                Min = 1
            };
            var expected = new ValidationResult(false, "Illegal characters: %");

            //Exercise
            var actual = target.Validate("%", null);

            //Verify
            Assert.AreEqual(expected.ErrorContent, actual.ErrorContent);
            Assert.AreEqual(expected.IsValid, actual.IsValid);
        }


        /// <summary>
        ///A test for Validate
        ///</summary>
        [TestMethod()]
        public void ValidateTest_ValidInput()
        {
            //Setup
            var target = new Int32RangeCheck()
            {
                Max = 100,
                Min = 1
            };
            var expected = ValidationResult.ValidResult;

            //Exercise
            var actual = target.Validate("5", null);

            //Verify
            Assert.AreEqual(expected, actual);
        }

    }
}
