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
    public class TotalCount_Should
    {
        private DbContextOptions<SmartDormitoryContext> contextOptions;

        [TestMethod]
        public async Task ReturnCorrectValue()
        {
            // Arrange
            contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
           .UseInMemoryDatabase(databaseName: "ReturnCorrectValue")
               .Options;

            using (var actContext = new SmartDormitoryContext(contextOptions))
            {
                await actContext.IcbSensors.AddRangeAsync(new IcbSensor
                {
                    Id = Guid.NewGuid().ToString(),
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
                await actContext.SaveChangesAsync();
            }

            // Act && Asert
            using (var assertContext = new SmartDormitoryContext(contextOptions))
            {
                var measureTypeServiceMock = new Mock<IMeasureTypeService>();
                var sut = new IcbSensorsService(assertContext, measureTypeServiceMock.Object);
                var result = await sut.TotalCount();

                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public async Task ReturnZero_IfNoSensors()
        {
            // Arrange
            contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
           .UseInMemoryDatabase(databaseName: "ReturnZero_IfNoSensors")
               .Options;

            // Act && Asert
            using (var assertContext = new SmartDormitoryContext(contextOptions))
            {
                var measureTypeServiceMock = new Mock<IMeasureTypeService>();
                var sut = new IcbSensorsService(assertContext, measureTypeServiceMock.Object);
                var result = await sut.TotalCount();

                Assert.AreEqual(0, result);
            }
        }
    }
}
