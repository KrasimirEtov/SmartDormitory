using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SmartDormitory.Services;
using SmartDormitory.Services.HttpClients;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace SmartDormitory.Tests.SmartDormitory.ServicesTests.IcbApiServices.Tests
{
    [TestClass]
    public class GetIcbSensorDataById_Should
    {
        [TestMethod]
        public async Task BubbleHttpRequestException_WhenIcbHttpClientThrows()
        {
            // Arrange
            var icbHttpClientMock = new Mock<IIcbHttpClient>();
            string validId = "1f0ef0ff-396b-40cb-ac3d-749196dee187";
            icbHttpClientMock.Setup(x => x.FetchSensorById(validId)).ThrowsAsync(new HttpRequestException());
            var sut = new IcbApiService(icbHttpClientMock.Object);
            // Act & Assert
            await Assert.ThrowsExceptionAsync<HttpRequestException>(
                () => sut.GetIcbSensorDataById(validId));
        }

        [TestMethod]
        public async Task ReturnCorectObject_WhenGetProperResponse()
        {
            // Arrange
            var validStringResponse = "{\n    \"timeStamp\": \"2018-12-16T14:59:25.2018078+02:00\",\n    \"value\": \"15.6\",\n    \"valueType\": \"°C\"\n}";
            string validId = "f1796a28-642e-401f-8129-fd7465417061";

            var icbHttpClientMock = new Mock<IIcbHttpClient>();
            icbHttpClientMock.Setup(x => x.FetchSensorById(validId)).Returns(Task.FromResult(validStringResponse));
            var sut = new IcbApiService(icbHttpClientMock.Object);

            // Act & Assert
            var result = await sut.GetIcbSensorDataById(validId);

            Assert.IsNotNull(result);
            Assert.IsTrue(result.LastValue.Equals("15.6") &&
                result.MeasurementUnit.Equals("°C") &&
                result.TimeStamp.Equals(DateTime.Parse("2018-12-16T14:59:25.2018078+02:00"))
                );
        }
    }
}
