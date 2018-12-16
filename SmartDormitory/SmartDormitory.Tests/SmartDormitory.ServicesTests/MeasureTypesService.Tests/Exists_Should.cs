using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SmartDormitory.App.Data;
using SmartDormitory.Data.Models;
using SmartDormitory.Services;
using System;
using System.Threading.Tasks;

namespace SmartDormitory.Tests.SmartDormitory.ServicesTests.MeasureTypesService.Tests
{
    [TestClass]
    public class Exists_Should
    {
        private DbContextOptions<SmartDormitoryContext> contextOptions;

        [TestMethod]
        public async Task ThrowArugmentNullException_WhenPassedNullSensorId()
        {
            // Arrange
            var contextMock = new Mock<SmartDormitoryContext>();

            var sut = new MeasureTypeService(contextMock.Object);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(
                () => sut.Exists(null), "Parameter id cannot be null!");
        }

        [TestMethod]
        public async Task ThrowArgumentException_WhenPassedInvalidGuid()
        {
            // Arrange
            var contextMock = new Mock<SmartDormitoryContext>();

            var sut = new MeasureTypeService(contextMock.Object);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => sut.Exists("invalidGuid"), "Parameter id is not a valid GUID!");
        }

        [TestMethod]
        public async Task ReturnTrue_WhenMeasureTypeExists()
        {
            // Arrange
            contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
           .UseInMemoryDatabase(databaseName: "ReturnTrue_WhenMeasureTypeExists")
               .Options;

            var existingId = Guid.NewGuid().ToString();
            var existingMeasureUnit = "ExistingMeasureUnit";
            var existingSensorType = "ExistingSensorType";
            var existingMeasureType = new MeasureType
            {
                Id = existingId,
                MeasureUnit = existingMeasureUnit,
                SuitableSensorType = existingSensorType,
                IsDeleted = false
            };

            using (var arrangeContext = new SmartDormitoryContext(contextOptions))
            {
                await arrangeContext.MeasureTypes.AddAsync(existingMeasureType);
                await arrangeContext.SaveChangesAsync();
            }

            // Act && Asert
            using (var assertContext = new SmartDormitoryContext(contextOptions))
            {
                var sut = new MeasureTypeService(assertContext);
                var result = await sut.Exists(existingId);

                Assert.IsTrue(result);
            }
        }

        [TestMethod]
        public async Task ReturnFalse_WhenMeasureTypeDoesntExists()
        {
            // Arrange
            contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
           .UseInMemoryDatabase(databaseName: "ReturnFalse_WhenMeasureTypeDoesntExists")
               .Options;

            // Act && Asert
            using (var assertContext = new SmartDormitoryContext(contextOptions))
            {
                var sut = new MeasureTypeService(assertContext);
                var result = await sut.Exists(Guid.NewGuid().ToString());

                Assert.IsFalse(result);
            }
        }
    }
}
