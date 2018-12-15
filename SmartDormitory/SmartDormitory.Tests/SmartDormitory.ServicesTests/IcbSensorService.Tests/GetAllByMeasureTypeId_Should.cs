using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SmartDormitory.App.Data;
using SmartDormitory.Data.Models;
using SmartDormitory.Services;
using SmartDormitory.Services.Contracts;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SmartDormitory.Tests.SmartDormitory.ServicesTests.IcbSensorService.Tests
{
    [TestClass]
    public class GetAllByMeasureTypeId_Should
    {
        private DbContextOptions<SmartDormitoryContext> contextOptions;

        [TestMethod]
        public async Task ReturnEmptyList_WhenPassedNullMeasureTypeId()
        {
            // Arrange
            contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
           .UseInMemoryDatabase(databaseName: "ReturnEmptyList_WhenPassedNullMeasureTypeId")
               .Options;

            // Act && Asert
            using (var assertContext = new SmartDormitoryContext(contextOptions))
            {
                var measureTypeServiceMock = new Mock<IMeasureTypeService>();
                var sut = new IcbSensorsService(assertContext, measureTypeServiceMock.Object);
                var result = await sut.GetAllByMeasureTypeId(measureTypeId: null);

                Assert.IsTrue(!result.Any());
            }
        }

        [TestMethod]
        public async Task ReturnEmptyList_WhenNoSensorsFoundByCriteria()
        {
            // Arrange
            contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
           .UseInMemoryDatabase(databaseName: "ReturnEmptyList_WhenNoSensorsFoundByCriteria")
               .Options;

            // Act && Asert
            using (var assertContext = new SmartDormitoryContext(contextOptions))
            {
                var measureTypeServiceMock = new Mock<IMeasureTypeService>();
                var sut = new IcbSensorsService(assertContext, measureTypeServiceMock.Object);

                var validGuidId = Guid.NewGuid().ToString();
                var result = await sut.GetAllByMeasureTypeId(measureTypeId: validGuidId);

                Assert.IsTrue(!result.Any());
            }
        }

        [TestMethod]
        public async Task ThrowArgumentException_WhenPassedInvalidGuid()
        {
            // Arrange
            var contextMock = new Mock<SmartDormitoryContext>();
            var measureTypeServiceMock = new Mock<IMeasureTypeService>();

            var sut = new IcbSensorsService(contextMock.Object, measureTypeServiceMock.Object);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => sut.GetAllByMeasureTypeId(measureTypeId: "invalidGuid"), "Parameter measureTypeId is not a valid GUID!");
        }

        [TestMethod]
        public async Task ReturnCorrectList_WhenPassedValidParams()
        {
            // Arrange
            contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
           .UseInMemoryDatabase(databaseName: "ReturnCorrectList_WhenPassedValidParams")
               .Options;

            var existingmeasureTypeId = Guid.NewGuid().ToString();
            var existingMeasureType = new MeasureType
            {
                Id = existingmeasureTypeId,
                MeasureUnit = "Existing unit",
                SuitableSensorType = "Some string"
            };

            var existingId = Guid.NewGuid().ToString();
            using (var assertContext = new SmartDormitoryContext(contextOptions))
            {
                await assertContext.MeasureTypes.AddAsync(existingMeasureType);
                await assertContext.IcbSensors.AddRangeAsync(new IcbSensor
                {
                    Id = existingId,
                    PollingInterval = 10,
                    Description = "Some description",
                    Tag = "Some tag",
                    MinRangeValue = 10,
                    MaxRangeValue = 20,
                    IsDeleted = false,
                    MeasureTypeId = existingmeasureTypeId
                },
                             new IcbSensor
                             {
                                 Id = Guid.NewGuid().ToString(),
                                 PollingInterval = 10,
                                 Description = "Some description",
                                 Tag = "Some tag",
                                 MinRangeValue = 10,
                                 MaxRangeValue = 20,
                                 IsDeleted = true,
                                 MeasureTypeId = existingmeasureTypeId
                             });
                await assertContext.SaveChangesAsync();
            }

            // Act && Asert
            using (var assertContext = new SmartDormitoryContext(contextOptions))
            {
                var measureTypeServiceMock = new Mock<IMeasureTypeService>();
                var sut = new IcbSensorsService(assertContext, measureTypeServiceMock.Object);
                var result = await sut.GetAllByMeasureTypeId(measureTypeId: existingmeasureTypeId);

                Assert.IsTrue(result.Count() == 1);
                Assert.IsTrue(result.Any(s => s.Id.Equals(existingId)));
            }
        }

        [TestMethod]
        public async Task ThrowArgumentOutOfRangeException_WhenPassedNegativePageValue()
        {
            // Arrange
            var contextMock = new Mock<SmartDormitoryContext>();
            var measureTypeServiceMock = new Mock<IMeasureTypeService>();

            var sut = new IcbSensorsService(contextMock.Object, measureTypeServiceMock.Object);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(
                () => sut.GetAllByMeasureTypeId(page: -1), "Parameter page must be bigger than 1!");
        }

        [TestMethod]
        public async Task ThrowArgumentOutOfRangeException_WhenPassedNegativePageSizeValue()
        {
            // Arrange
            var contextMock = new Mock<SmartDormitoryContext>();
            var measureTypeServiceMock = new Mock<IMeasureTypeService>();

            var sut = new IcbSensorsService(contextMock.Object, measureTypeServiceMock.Object);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentOutOfRangeException>(
                () => sut.GetAllByMeasureTypeId(pageSize: -1), "Parameter pageSize must be bigger than 0!");
        }
    }
}
