using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
    public class Create_Should
    {
        private DbContextOptions<SmartDormitoryContext> contextOptions;

        [TestMethod]
        public async Task ThrowsEntityAlreadyExistsException_IfMeasureTypeExists()
        {
            // Arrange
            contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
           .UseInMemoryDatabase(databaseName: "ThrowsEntityAlreadyExistsException_IfMeasureTypeExists")
               .Options;

            var existingId = Guid.NewGuid().ToString();
            var existingMeasureUnit = "Some existing measure unit";
            var existingSuitableSensorType = "Some existing sensor type";
            var softDeletedMeasureType = new MeasureType
            {
                Id = existingId,
                MeasureUnit = existingMeasureUnit,
                SuitableSensorType = existingSuitableSensorType,
                IsDeleted = false
            };

            using (var arrangeContext = new SmartDormitoryContext(contextOptions))
            {
                await arrangeContext.MeasureTypes.AddAsync(softDeletedMeasureType);
                await arrangeContext.SaveChangesAsync();
            }

            // Act && Asert
            using (var assertContext = new SmartDormitoryContext(contextOptions))
            {
                var sut = new MeasureTypeService(assertContext);

                await Assert.ThrowsExceptionAsync<EntityAlreadyExistsException>(
                    () => sut.Create(existingMeasureUnit, existingSuitableSensorType), "\nMeasure type is already present in the database.");
            }
        }

        [TestMethod]
        public async Task RestoreMeasureType_IfItIsSoftDeleted()
        {
            // Arrange
            contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
           .UseInMemoryDatabase(databaseName: "RestoreMeasureType_IfItIsSoftDeleted")
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

                await sut.Create(deletedMeasureUnit, deletedSuitableSensorType);

                Assert.IsTrue(assertContext.MeasureTypes.Any(mt => mt.Id == deletedId && mt.IsDeleted == false));
            }
        }

        [TestMethod]
        public async Task AddMeasureType_IfDoesntExists()
        {
            // Arrange
            contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
           .UseInMemoryDatabase(databaseName: "AddMeasureType_IfDoesntExists")
               .Options;

            var newMeasureUnit = "New measure unit";
            var newSuitableSensorType = "New sensor type";

            // Act && Asert
            using (var assertContext = new SmartDormitoryContext(contextOptions))
            {
                var sut = new MeasureTypeService(assertContext);

                await sut.Create(newMeasureUnit, newSuitableSensorType);

                Assert.IsTrue(assertContext.MeasureTypes.Count() == 1);
                Assert.IsTrue(assertContext.MeasureTypes.Any(mt => mt.MeasureUnit == newMeasureUnit && mt.SuitableSensorType == newSuitableSensorType));
            }
        }
    }
}
