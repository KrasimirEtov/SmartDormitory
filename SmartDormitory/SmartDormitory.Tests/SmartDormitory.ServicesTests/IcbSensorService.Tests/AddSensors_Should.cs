using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SmartDormitory.App.Data;
using SmartDormitory.Data.Models;
using SmartDormitory.Services;
using SmartDormitory.Services.Contracts;
using SmartDormitory.Services.Models.JsonDtoModels;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartDormitory.Tests.SmartDormitory.ServicesTests.IcbSensorService.Tests
{
    [TestClass]
    public class AddSensors_Should
    {
        private DbContextOptions<SmartDormitoryContext> contextOptions;

        [TestMethod]
        public async Task NotAddSensor_WhenPassedInvalidMeasureType()
        {
            // Arrange
            contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
           .UseInMemoryDatabase(databaseName: " NotAddSensor_WhenPassedInvalidMeasureType")
               .Options;

            var newSensor = new ApiSensorDetailsDTO
            {
                ApiSensorId = Guid.NewGuid().ToString(),
                MinPollingIntervalInSeconds = 10,
                Description = "Some description",
                Tag = "Some tag",
                MeasureType = "Inexisting measure type"
            };

            var sensorsToAdd = new List<ApiSensorDetailsDTO>() { newSensor };

            // Act and assert
            using (var actContext = new SmartDormitoryContext(contextOptions))
            {
                var measureTypeServiceMock = new Mock<IMeasureTypeService>();
                var sut = new IcbSensorsService(actContext, measureTypeServiceMock.Object);

                await sut.AddSensors(sensorsToAdd);
                var actualCount = await actContext.IcbSensors.CountAsync();

                Assert.AreEqual(0, actualCount);
            }
        }

        [TestMethod]
        public async Task AddSensor_WhenPassedValidMeasureType()
        {
            // Arrange
            contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
           .UseInMemoryDatabase(databaseName: "AddSensor_WhenPassedValidMeasureType")
               .Options;

            var existingmeasureTypeId = Guid.NewGuid().ToString();
            var existingMeasureType = new MeasureType
            {
                Id = existingmeasureTypeId,
                MeasureUnit = "Existing unit",
                SuitableSensorType = "Some string"
            };

            string newApiSensorId = Guid.NewGuid().ToString();
            var newApiSensor = new ApiSensorDetailsDTO
            {
                ApiSensorId = newApiSensorId,
                MinPollingIntervalInSeconds = 10,
                Description = "Some description",
                Tag = "Some tag",
                MeasureType = "Existing unit"
            };

            var sensorsToAdd = new List<ApiSensorDetailsDTO>() { newApiSensor };

            using (var arrangeContext = new SmartDormitoryContext(contextOptions))
            {
                await arrangeContext.MeasureTypes.AddAsync(existingMeasureType);
                await arrangeContext.SaveChangesAsync();
            }

            // Act and assert
            using (var actContext = new SmartDormitoryContext(contextOptions))
            {
                var measureTypeServiceMock = new Mock<IMeasureTypeService>();
                var sut = new IcbSensorsService(actContext, measureTypeServiceMock.Object);

                await sut.AddSensors(sensorsToAdd);
                var actualCount = await actContext.IcbSensors.CountAsync();

                Assert.AreEqual(1, actualCount);
            }
        }

        [TestMethod]
        public async Task AddSensorWithCorrectMeasureType_WhenPassedValidMeasureType()
        {
            // Arrange
            contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
           .UseInMemoryDatabase(databaseName: "AddSensorWithCorrectMeasureType_WhenPassedValidMeasureType")
               .Options;

            var existingMeasureTypeId = Guid.NewGuid().ToString();
            var existingMeasureType = new MeasureType
            {
                Id = existingMeasureTypeId,
                MeasureUnit = "Existing unit",
                SuitableSensorType = "Some string"
            };

            string newApiSensorId = Guid.NewGuid().ToString();
            var newApiSensor = new ApiSensorDetailsDTO
            {
                ApiSensorId = newApiSensorId,
                MinPollingIntervalInSeconds = 10,
                Description = "Some description",
                Tag = "Some tag",
                MeasureType = "Existing unit"
            };

            var sensorsToAdd = new List<ApiSensorDetailsDTO>() { newApiSensor };

            using (var arrangeContext = new SmartDormitoryContext(contextOptions))
            {
                await arrangeContext.MeasureTypes.AddAsync(existingMeasureType);
                await arrangeContext.SaveChangesAsync();
            }

            // Act and assert
            using (var actContext = new SmartDormitoryContext(contextOptions))
            {
                var measureTypeServiceMock = new Mock<IMeasureTypeService>();
                var sut = new IcbSensorsService(actContext, measureTypeServiceMock.Object);

                await sut.AddSensors(sensorsToAdd);

                Assert.IsTrue(await actContext.IcbSensors.AnyAsync(s => s.MeasureTypeId == existingMeasureTypeId));
            }
        }

        [TestMethod]
        public async Task AddCorrectSensor_WhenPassedValidMeasureType()
        {
            // Arrange
            contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
           .UseInMemoryDatabase(databaseName: "AddCorrectSensor_WhenPassedValidMeasureType")
               .Options;

            var existingmeasureTypeId = Guid.NewGuid().ToString();
            var existingMeasureType = new MeasureType
            {
                Id = existingmeasureTypeId,
                MeasureUnit = "Existing unit",
                SuitableSensorType = "Some string"
            };

            string newApiSensorId = Guid.NewGuid().ToString();
            int newApiSensorPollingInterval = 10;
            string newApiSensorDescription = "Some description";
            string newApiSensorTag = "Some tag";
            var newApiSensor = new ApiSensorDetailsDTO
            {
                ApiSensorId = newApiSensorId,
                MinPollingIntervalInSeconds = newApiSensorPollingInterval,
                Description = newApiSensorDescription,
                Tag = newApiSensorTag,
                MeasureType = "Existing unit"
            };

            var sensorsToAdd = new List<ApiSensorDetailsDTO>() { newApiSensor };

            using (var arrangeContext = new SmartDormitoryContext(contextOptions))
            {
                await arrangeContext.MeasureTypes.AddAsync(existingMeasureType);
                await arrangeContext.SaveChangesAsync();
            }

            // Act and assert
            using (var actContext = new SmartDormitoryContext(contextOptions))
            {
                var measureTypeServiceMock = new Mock<IMeasureTypeService>();
                var sut = new IcbSensorsService(actContext, measureTypeServiceMock.Object);

                await sut.AddSensors(sensorsToAdd);

                var createdSensorId = actContext
                    .IcbSensors
                    .FirstOrDefaultAsync(s => s.Id == newApiSensorId &&
                    s.Description == newApiSensorDescription &&
                    s.Tag == newApiSensorTag &&
                    s.PollingInterval == newApiSensorPollingInterval &&
                    s.MeasureTypeId == existingmeasureTypeId);

                Assert.IsNotNull(createdSensorId);
            }
        }
    }
}
