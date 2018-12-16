using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartDormitory.Services.Utils.Helpers;

namespace SmartDormitory.Tests.SmartDormitory.ServicesTests.Utils.Helpers.ApiDataHelpers.Tests
{
    [TestClass]
    public class GetMinAndMaxRange_Should
    {
        [TestMethod]
        [DataRow("This sensor will return values between 500 and 3500")]
        public void Return_CorrectMinMaxValues_WhenPassedDigitsAsString(string description)
        {
            var result = ApiDataHelper.GetMinAndMaxRange(description);

            Assert.AreEqual(500, result.MinRange);
            Assert.AreEqual(3500, result.MaxRange);
        }

        [TestMethod]
        [DataRow("This sensor will return true or false value")]
        public void Return_CorrectMinMaxValues_WhenPassedBooleansAsString(string description)
        {
            var result = ApiDataHelper.GetMinAndMaxRange(description);

            Assert.AreEqual(0, result.MinRange);
            Assert.AreEqual(1, result.MaxRange);
        }
    }
}
