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
    public class TotalCount_Should
    {
        private DbContextOptions<SmartDormitoryContext> contextOptions;

        [TestMethod]
        public async Task Return_CorrectMeasureTypeCount()
        {
            // Arrange
            contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
           .UseInMemoryDatabase(databaseName: "Return_CorrectMeasureTypeCount")
               .Options;

            var deletedId = Guid.NewGuid().ToString();

            using (var arrangeContext = new SmartDormitoryContext(contextOptions))
            {
                await arrangeContext.MeasureTypes.AddRangeAsync(
                    new MeasureType
                    {
                        Id = deletedId,
                        MeasureUnit = "Some measure unit",
                        SuitableSensorType = "Some description",
                        IsDeleted = false
                    },
                    new MeasureType
                    {
                        Id = Guid.NewGuid().ToString(),
                        MeasureUnit = "Some soft deleted measure unit",
                        SuitableSensorType = "Some soft deleted description",
                        IsDeleted = true
                    });
                await arrangeContext.SaveChangesAsync();
            }

            // Act && Asert
            using (var assertContext = new SmartDormitoryContext(contextOptions))
            {
                var sut = new MeasureTypeService(assertContext);
                var result = await sut.TotalCount();

                Assert.AreEqual(1, result);
            }
        }

        [TestMethod]
        public async Task ReturnZero_IfNoMeasureTypes()
        {
            // Arrange
            contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
           .UseInMemoryDatabase(databaseName: "ReturnZero_IfNoMeasureTypes")
               .Options;

            // Act && Asert
            using (var assertContext = new SmartDormitoryContext(contextOptions))
            {
                var sut = new MeasureTypeService(assertContext);
                var result = await sut.TotalCount();

                Assert.AreEqual(0, result);
            }
        }
    }
}
