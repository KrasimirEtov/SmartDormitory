using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartDormitory.Services.Utils.Helpers;
using System;

namespace SmartDormitory.Tests.SmartDormitory.ServicesTests.Utils.Helpers.ApiDataHelpers.Tests
{
    [TestClass]
    public class GetLastValue_Should
    {
        [TestMethod]
        [DataRow("16.5")]
        public void Return_CorrectFloatValue(string stringText)
        {
            var result = ApiDataHelper.GetLastValue(stringText);

            Assert.AreEqual(16.5, result);
        }

        [TestMethod]
        [DataRow("true")]
        public void Return_CorrectValue_WhenPassedTrueAsString(string stringText)
        {
            var result = ApiDataHelper.GetLastValue(stringText);

            Assert.AreEqual(1, result);
        }

        [TestMethod]
        [DataRow("false")]
        public void Return_CorrectValue_WhenPassedFalseAsString(string stringText)
        {
            var result = ApiDataHelper.GetLastValue(stringText);

            Assert.AreEqual(0, result);
        }

        [TestMethod]
        [DataRow("invalid string")]
        public void ThrowsInvalidOperationException_WhenPassedInvalidString(string stringText)
        {
            Assert.ThrowsException<InvalidOperationException>(() => ApiDataHelper.GetLastValue(stringText), "Invalid last value response");
        }
    }
}
