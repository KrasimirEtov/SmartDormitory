using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SmartDormitory.App.Data;
using SmartDormitory.Data.Models;
using SmartDormitory.Services;
using SmartDormitory.Services.Contracts;
using System;
using System.Threading.Tasks;

namespace SmartDormitory.Tests.SmartDormitory.ServicesTests.IcbSensorService.Tests
{
    [TestClass]
    public class ExistsById_Should
    {
        private DbContextOptions<SmartDormitoryContext> contextOptions;

        [TestMethod]
        public async Task ReturnTrue_WhenSensorExists()
        {
            // Arrange
            contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
           .UseInMemoryDatabase(databaseName: "ReturnTrue_WhenSensorExists")
               .Options;

            var existingId = Guid.NewGuid().ToString();
            using (var assertContext = new SmartDormitoryContext(contextOptions))
            {
                await assertContext.IcbSensors.AddRangeAsync(new IcbSensor
                {
                    Id = existingId,
                    PollingInterval = 10,
                    Description = "Some description",
                    Tag = "Some tag",
                    MinRangeValue = 10,
                    MaxRangeValue = 20,
                    IsDeleted = false
                },
                new IcbSensor
                {
                    Id = Guid.NewGuid().ToString(),
                    PollingInterval = 10,
                    Description = "Some description",
                    Tag = "Some tag",
                    MinRangeValue = 10,
                    MaxRangeValue = 20,
                    IsDeleted = true
                });
                await assertContext.SaveChangesAsync();
            }

            // Act && Asert
            using (var assertContext = new SmartDormitoryContext(contextOptions))
            {
                var measureTypeServiceMock = new Mock<IMeasureTypeService>();
                var sut = new IcbSensorsService(assertContext, measureTypeServiceMock.Object);
                var result = await sut.ExistsById(existingId);

                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ThrowArugmentNullException_WhenPassedNullSensorId()
        {
            // Arrange
            var contextMock = new Mock<SmartDormitoryContext>();
            var measureTypeServiceMock = new Mock<IMeasureTypeService>();

            var sut = new IcbSensorsService(contextMock.Object, measureTypeServiceMock.Object);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(
                () => sut.GetById(null), "Parameter sensorId cannot be null!");
        }

        [TestMethod]
        public async Task ThrowArugmentException_WhenPassedInvalidGuid()
        {
            // Arrange
            var contextMock = new Mock<SmartDormitoryContext>();
            var measureTypeServiceMock = new Mock<IMeasureTypeService>();

            var sut = new IcbSensorsService(contextMock.Object, measureTypeServiceMock.Object);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => sut.GetById("invalidGuid"), "Parameter sensorId is not a valid GUID!");
        }
    }
}
