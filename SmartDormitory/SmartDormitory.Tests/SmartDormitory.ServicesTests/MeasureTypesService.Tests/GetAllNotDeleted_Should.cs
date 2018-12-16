using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartDormitory.App.Data;
using SmartDormitory.Data.Models;
using SmartDormitory.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SmartDormitory.Tests.SmartDormitory.ServicesTests.MeasureTypesService.Tests
{
    [TestClass]
    public class GetAllNotDeleted_Should
    {
        private DbContextOptions<SmartDormitoryContext> contextOptions;

        [TestMethod]
        public async Task Return_CorrectList()
        {
            // Arrange
            contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
           .UseInMemoryDatabase(databaseName: "Return_CorrectList")
               .Options;

            var existingId = Guid.NewGuid().ToString();
            using (var arrangeContext = new SmartDormitoryContext(contextOptions))
            {
                await arrangeContext.MeasureTypes.AddRangeAsync(
                    new MeasureType
                    {
                        Id = existingId,
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
                var result = await sut.GetAllNotDeleted();

                Assert.IsTrue(result.Count() == 1);
                Assert.IsTrue(result.Any(mt => mt.Id == existingId));
            }
        }

        [TestMethod]
        public async Task ReturnEmptyList_WhenOnlySoftDeletedItems()
        {
            // Arrange
            contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
           .UseInMemoryDatabase(databaseName: "ReturnEmptyList_WhenOnlySoftDeletedItems")
               .Options;

            using (var arrangeContext = new SmartDormitoryContext(contextOptions))
            {
                await arrangeContext.MeasureTypes.AddRangeAsync(
                    new MeasureType
                    {
                        Id = Guid.NewGuid().ToString(),
                        MeasureUnit = "Some measure unit",
                        SuitableSensorType = "Some description",
                        IsDeleted = true
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
                var result = await sut.GetAllNotDeleted();

                Assert.IsTrue(result.Count() == 0);
            }
        }
    }
}
