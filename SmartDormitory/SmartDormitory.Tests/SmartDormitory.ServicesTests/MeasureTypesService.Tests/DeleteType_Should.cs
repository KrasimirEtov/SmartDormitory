using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SmartDormitory.App.Data;
using SmartDormitory.Data.Models;
using SmartDormitory.Services;
using SmartDormitory.Services.Exceptions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SmartDormitory.Tests.SmartDormitory.ServicesTests.MeasureTypesService.Tests
{
    [TestClass]
    public class DeleteType_Should
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
                () => sut.GetType(null), "Parameter typeId cannot be null!");
        }

        [TestMethod]
        public async Task ThrowArgumentException_WhenPassedInvalidGuid()
        {
            // Arrange
            var contextMock = new Mock<SmartDormitoryContext>();

            var sut = new MeasureTypeService(contextMock.Object);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => sut.GetType("invalidGuid"), "Parameter typeId is not a valid GUID!");
        }

        [TestMethod]
        public async Task ThrowsEntityDoesntExistException_WhenDoesntExists()
        {
            // Arrange
            contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
           .UseInMemoryDatabase(databaseName: "ThrowsEntityDoesntExistException_WhenDoesntExists")
               .Options;

            var inexistingId = Guid.NewGuid().ToString();

            // Act && Asert
            using (var assertContext = new SmartDormitoryContext(contextOptions))
            {
                var sut = new MeasureTypeService(assertContext);

                await Assert.ThrowsExceptionAsync<EntityDoesntExistException>(
                    () => sut.DeleteType(inexistingId), "\nMeasure Type doesn't exists!");
            }
        }

        [TestMethod]
        public async Task SoftDelete_WhenMeasureTypeExists()
        {
            // Arrange
            contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
           .UseInMemoryDatabase(databaseName: "SoftDelete_WhenMeasureTypeExists")
               .Options;

            var existingId = Guid.NewGuid().ToString();
            var existingMeasureUnit = "Some existing measure unit";
            var existingSuitableSensorType = "Some existing sensor type";
            using (var arrangeContext = new SmartDormitoryContext(contextOptions))
            {
                await arrangeContext.MeasureTypes.AddAsync(
                    new MeasureType
                    {
                        Id = existingId,
                        MeasureUnit = existingMeasureUnit,
                        SuitableSensorType = existingSuitableSensorType,
                        IsDeleted = false
                    });
                await arrangeContext.SaveChangesAsync();
            }

            // Act && Asert
            using (var assertContext = new SmartDormitoryContext(contextOptions))
            {
                var sut = new MeasureTypeService(assertContext);

                await sut.DeleteType(existingId);

                Assert.IsTrue(assertContext.MeasureTypes.Any(mt => mt.Id == existingId && mt.IsDeleted == true));
            }
        }

        [TestMethod]
        public async Task SkipMeasureType_WhenItIsSoftDeleted()
        {
            // Arrange
            contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
           .UseInMemoryDatabase(databaseName: "SkipMeasureType_WhenItIsSoftDeleted")
               .Options;

            var deletedId = Guid.NewGuid().ToString();
            var deletedMeasureUnit = "Some soft deleted measure unit";
            var deletedSuitableSensorType = "Some soft deleted sensor type";
            using (var arrangeContext = new SmartDormitoryContext(contextOptions))
            {
                await arrangeContext.MeasureTypes.AddAsync(
                    new MeasureType
                    {
                        Id = deletedId,
                        MeasureUnit = deletedMeasureUnit,
                        SuitableSensorType = deletedSuitableSensorType,
                        IsDeleted = true
                    });
                await arrangeContext.SaveChangesAsync();
            }

            // Act && Asert
            using (var assertContext = new SmartDormitoryContext(contextOptions))
            {
                var sut = new MeasureTypeService(assertContext);

                await sut.DeleteType(deletedId);

                Assert.IsTrue(assertContext.MeasureTypes.Any(mt => mt.Id == deletedId && mt.IsDeleted == false));
            }
        }
    }
}
