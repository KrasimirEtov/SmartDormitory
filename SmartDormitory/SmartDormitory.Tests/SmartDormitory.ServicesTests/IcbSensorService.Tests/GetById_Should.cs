using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SmartDormitory.App.Data;
using SmartDormitory.Data.Models;
using SmartDormitory.Services;
using SmartDormitory.Services.Contracts;
using SmartDormitory.Services.Models.IcbSensors;
using System;
using System.Threading.Tasks;

namespace SmartDormitory.Tests.SmartDormitory.ServicesTests.IcbSensorService.Tests
{
    [TestClass]
    public class GetById_Should
    {
        private DbContextOptions<SmartDormitoryContext> contextOptions;

        [TestMethod]
        public async Task Return_Null_When_Id_Is_Not_Found()
        {
            // Arrange
            contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
            .UseInMemoryDatabase(databaseName: "Return_Null_When_Id_Is_Not_Found")
                .Options;
            string id = Guid.NewGuid().ToString(); ;

            // Act && Asert
            using (var assertContext = new SmartDormitoryContext(contextOptions))
            {
                var icbApiServiceMock = new Mock<IIcbApiService>();
                var measureTypeServiceMock = new Mock<IMeasureTypeService>();
                var icbSensorService = new IcbSensorsService(assertContext, icbApiServiceMock.Object, measureTypeServiceMock.Object);
                var result = await icbSensorService.GetById(id);

                Assert.IsNull(result);
            }
        }

        [TestMethod]
        public async Task Return_CorrectIcbSensor_WhenFoundId()
        {
            // Arrange
            contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
           .UseInMemoryDatabase(databaseName: "Return_CorrectUser_WhenFoundId")
               .Options;
            string id = Guid.NewGuid().ToString();

            using (var assertContext = new SmartDormitoryContext(contextOptions))
            {
                await assertContext.IcbSensors.AddAsync(new IcbSensor
                {
                    Id = id,
                    PollingInterval = 10,
                    Description = "Some description",
                    Tag = "Some tag",
                    MinRangeValue = 10,
                    MaxRangeValue = 20
                });
                await assertContext.SaveChangesAsync();
            }

            // Act && Asert
            using (var assertContext = new SmartDormitoryContext(contextOptions))
            {
                var icbApiServiceMock = new Mock<IIcbApiService>();
                var measureTypeServiceMock = new Mock<IMeasureTypeService>();
                var icbSensorService = new IcbSensorsService(assertContext, icbApiServiceMock.Object, measureTypeServiceMock.Object);
                var result = await icbSensorService.GetById(id);

                Assert.IsNotNull(result);
                Assert.IsTrue(id.Equals(result.Id));
            }
        }

        [TestMethod]
        public async Task Return_CorrectType_WhenFoundId()
        {
            // Arrange
            contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
           .UseInMemoryDatabase(databaseName: "Return_CorrectType_WhenFoundId")
               .Options;
            string id = Guid.NewGuid().ToString();

            using (var assertContext = new SmartDormitoryContext(contextOptions))
            {
                await assertContext.IcbSensors.AddRangeAsync(new IcbSensor
                {
                    Id = id,
                    PollingInterval = 10,
                    Description = "Some description",
                    Tag = "Some tag",
                    MinRangeValue = 10,
                    MaxRangeValue = 20,
                    IsDeleted = false,
                },
                new IcbSensor
                {
                    Id = "deletedId",
                    PollingInterval = 10,
                    Description = "Some description",
                    Tag = "Some tag",
                    MinRangeValue = 10,
                    MaxRangeValue = 20,
                    IsDeleted = true,
                });
                await assertContext.SaveChangesAsync();
            }

            // Act && Asert
            using (var assertContext = new SmartDormitoryContext(contextOptions))
            {
                var icbApiServiceMock = new Mock<IIcbApiService>();
                var measureTypeServiceMock = new Mock<IMeasureTypeService>();
                var icbSensorService = new IcbSensorsService(assertContext, icbApiServiceMock.Object, measureTypeServiceMock.Object);

                var result = await icbSensorService.GetById(id);

                Assert.IsInstanceOfType(result, typeof(IcbSensorCreateServiceModel));
            }
        }

        [TestMethod]
        public async Task ThrowArugmentNullException_WhenPassedNullSensorId()
        {
            // Arrange
            var contextMock = new Mock<SmartDormitoryContext>();
            var icbApiServiceMock = new Mock<IIcbApiService>();
            var measureTypeServiceMock = new Mock<IMeasureTypeService>();

            var icbSensorService = new IcbSensorsService(contextMock.Object, icbApiServiceMock.Object, measureTypeServiceMock.Object);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(
                () => icbSensorService.GetById(null), "Parameter sensorId cannot be null!");
        }

        [TestMethod]
        public async Task ThrowArugmentException_WhenPassedInvalidGuid()
        {
            // Arrange
            var contextMock = new Mock<SmartDormitoryContext>();
            var icbApiServiceMock = new Mock<IIcbApiService>();
            var measureTypeServiceMock = new Mock<IMeasureTypeService>();

            var icbSensorService = new IcbSensorsService(contextMock.Object, icbApiServiceMock.Object, measureTypeServiceMock.Object);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => icbSensorService.GetById("invalidGuid"), "Parameter sensorId is not a valid GUID!");
        }
    }
}
