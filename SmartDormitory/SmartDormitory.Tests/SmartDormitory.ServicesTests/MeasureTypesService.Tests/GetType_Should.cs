using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SmartDormitory.App.Data;
using SmartDormitory.Data.Models;
using SmartDormitory.Services;
using SmartDormitory.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartDormitory.Tests.SmartDormitory.ServicesTests.MeasureTypesService.Tests
{
    [TestClass]
    public class GetType_Should
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
        public async Task Return_CorrectMeasureTypeById()
        {
            // Arrange
            contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
           .UseInMemoryDatabase(databaseName: "Return_CorrectMeasureTypeById")
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
                var result = await sut.GetType(existingId);

                Assert.IsTrue(result.Id == existingId &&
                    result.MeasureUnit == existingMeasureUnit &&
                    result.SuitableSensorType == existingSensorType);
            }
        }

        [TestMethod]
        public async Task ReturnNull_IfNosuchMeasureType()
        {
            // Arrange
            contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
           .UseInMemoryDatabase(databaseName: "ReturnNull_IfNosuchMeasureType")
               .Options;

            // Act && Asert
            using (var assertContext = new SmartDormitoryContext(contextOptions))
            {
                var sut = new MeasureTypeService(assertContext);
                var result = await sut.GetMeasureType("Some inexisting measure unit", "Some inexisting senstor type");

                Assert.IsNull(result);
            }
        }
    }
}
