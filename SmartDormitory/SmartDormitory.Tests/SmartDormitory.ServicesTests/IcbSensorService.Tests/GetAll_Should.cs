using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SmartDormitory.App.Data;
using SmartDormitory.Data.Models;
using SmartDormitory.Services;
using SmartDormitory.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartDormitory.Tests.SmartDormitory.ServicesTests.IcbSensorService.Tests
{
    [TestClass]
    public class GetAll_Should
    {
        private DbContextOptions<SmartDormitoryContext> contextOptions;

        [TestMethod]
        public async Task ReturnCorrectSensorsCount_WhenCalled()
        {
            // Arrange
            contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
           .UseInMemoryDatabase(databaseName: "ReturnCorrectSensorsCount_WhenCalled")
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
                });
                await assertContext.SaveChangesAsync();
            }

            // Act && Asert
            using (var assertContext = new SmartDormitoryContext(contextOptions))
            {
                var measureTypeServiceMock = new Mock<IMeasureTypeService>();
                var icbSensorService = new IcbSensorsService(assertContext, measureTypeServiceMock.Object);
                IEnumerable<IcbSensor> result = await icbSensorService.GetAll();

                Assert.IsTrue(result.Count() == 1);
            }
        }

        [TestMethod]
        public async Task ReturnZero_WhenHaveOnlySoftDeletedSensors()
        {
            // Arrange
            contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
           .UseInMemoryDatabase(databaseName: "ReturnZero_WhenHaveOnlySoftDeletedSensors")
               .Options;

            var existingId = Guid.NewGuid().ToString();
            using (var assertContext = new SmartDormitoryContext(contextOptions))
            {
                await assertContext
                        .IcbSensors
                        .AddRangeAsync(new IcbSensor
                        {
                            Id = existingId,
                            PollingInterval = 10,
                            Description = "Some description",
                            Tag = "Some tag",
                            MinRangeValue = 10,
                            MaxRangeValue = 20,
                            IsDeleted = true
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
                var icbSensorService = new IcbSensorsService(assertContext, measureTypeServiceMock.Object);
                IEnumerable<IcbSensor> result = await icbSensorService.GetAll();

                Assert.IsTrue(result.Count() == 0);
            }
        }
    }
}
