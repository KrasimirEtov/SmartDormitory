using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartDormitory.App.Data;
using SmartDormitory.Data.Models;
using SmartDormitory.Services;
using System;
using System.Threading.Tasks;

namespace SmartDormitory.Tests.SmartDormitory.ServicesTests.MeasureTypesService.Tests
{
    [TestClass]
    public class GetMeasureType_Should
    {
        private DbContextOptions<SmartDormitoryContext> contextOptions;

        [TestMethod]
        public async Task Return_CorrectMeasureTypeBy()
        {
            // Arrange
            contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
           .UseInMemoryDatabase(databaseName: "Return_CorrectMeasureTypeBy")
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
                await arrangeContext.MeasureTypes.AddRangeAsync(existingMeasureType);
                await arrangeContext.SaveChangesAsync();
            }

            // Act && Asert
            using (var assertContext = new SmartDormitoryContext(contextOptions))
            {
                var sut = new MeasureTypeService(assertContext);
                var result = await sut.GetMeasureType(existingMeasureUnit, existingSensorType);

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
