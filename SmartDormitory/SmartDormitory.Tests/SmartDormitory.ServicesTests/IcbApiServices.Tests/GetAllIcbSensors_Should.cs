using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SmartDormitory.Services;
using SmartDormitory.Services.HttpClients;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace SmartDormitory.Tests.SmartDormitory.ServicesTests.IcbApiServices.Tests
{
    [TestClass]
    public class GetAllIcbSensors_Should
    {
        [TestMethod]
        public async Task BubbleHttpRequestException_WhenIcbHttpClientThrows()
        {
            // Arrange
            var icbHttpClientMock = new Mock<IIcbHttpClient>();
            icbHttpClientMock.Setup(x => x.FetchAllSensors()).ThrowsAsync(new HttpRequestException());
            var sut = new IcbApiService(icbHttpClientMock.Object);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<HttpRequestException>(
                () => sut.GetAllIcbSensors());
        }

        [TestMethod]
        public async Task ReturnValidList_WhenGetProperResponse()
        {
            // Arrange
            var validStringResponse = @"[
    {
                ""sensorId"": ""f1796a28-642e-401f-8129-fd7465417061"",
        ""tag"": ""TemperatureSensor1"",
        ""description"": ""This sensor will return values between 15 and 28"",
        ""minPollingIntervalInSeconds"": 40,
        ""measureType"": ""°C""
    }]";
            var icbHttpClientMock = new Mock<IIcbHttpClient>();
            icbHttpClientMock.Setup(x => x.FetchAllSensors()).Returns(Task.FromResult(validStringResponse));
            var sut = new IcbApiService(icbHttpClientMock.Object);

            // Act & Assert
            var result = await sut.GetAllIcbSensors();
            Assert.IsTrue(result.Count() == 1);
        }
    }
}
