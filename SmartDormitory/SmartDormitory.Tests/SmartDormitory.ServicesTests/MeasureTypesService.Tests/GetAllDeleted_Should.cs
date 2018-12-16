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
    public class GetAllDeleted_Should
    {
        private DbContextOptions<SmartDormitoryContext> contextOptions;

        [TestMethod]
        public async Task Return_CorrectList()
        {
            // Arrange
            contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
           .UseInMemoryDatabase(databaseName: "Return_CorrectListAllDeleted")
               .Options;

            var deletedId = Guid.NewGuid().ToString();
            var deletedId2 = Guid.NewGuid().ToString();

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
                        Id = deletedId2,
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
                var result = await sut.GetAllDeleted();

                Assert.IsTrue(result.Count() == 2);
                Assert.IsTrue(result.Any(mt => mt.Id == deletedId));
                Assert.IsTrue(result.Any(mt => mt.Id == deletedId2));
            }
        }

        [TestMethod]
        public async Task ReturnEmptyList_WhenNoValidRecords()
        {
            // Arrange
            contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
           .UseInMemoryDatabase(databaseName: "ReturnEmptyList_WhenNoValidRecords")
               .Options;

            // Act && Asert
            using (var assertContext = new SmartDormitoryContext(contextOptions))
            {
                var sut = new MeasureTypeService(assertContext);
                var result = await sut.GetAllDeleted();

                Assert.IsTrue(!result.Any());
            }
        }
    }
}
