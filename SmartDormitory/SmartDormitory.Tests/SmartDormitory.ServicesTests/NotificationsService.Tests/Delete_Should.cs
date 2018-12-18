using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SmartDormitory.App.Data;
using SmartDormitory.Data.Models;
using SmartDormitory.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SmartDormitory.Tests.SmartDormitory.ServicesTests.NotificationsService.Tests
{
    [TestClass]
    public class Delete_Should
    {
        private DbContextOptions<SmartDormitoryContext> contextOptions;

        [TestMethod]
        public async Task ThrowArugmentNullException_WhenPassedNullId()
        {
            // Arrange
            var contextMock = new Mock<SmartDormitoryContext>();

            var sut = new NotificationService(contextMock.Object);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(
                () => sut.Delete(null), "Parameter id cannot be null!");
        }

        [TestMethod]
        public async Task Throw_ArugmentException_When_Passed_Invalid_Guid()
        {
            // Arrange
            var contextMock = new Mock<SmartDormitoryContext>();

            var sut = new NotificationService(contextMock.Object);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => sut.Delete("InvalidGuid"));
        }

        [TestMethod]
        public async Task DeleteNotification_WhenPassedValidId()
        {
            // Arrange
            contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
           .UseInMemoryDatabase(databaseName: "DeleteNotification_WhenPassedValidId")
               .Options;

            var existingId = Guid.NewGuid().ToString();
            using (var assertContext = new SmartDormitoryContext(contextOptions))
            {
                await assertContext.Notifications.AddRangeAsync(new Notification
                {
                    Id = existingId,
                    Message = "Some message",
                    Title = "Some title",
                    IsDeleted = false,
                });
                await assertContext.SaveChangesAsync();
            }

            // Act && Asert
            using (var assertContext = new SmartDormitoryContext(contextOptions))
            {
                var sut = new NotificationService(assertContext);
                await sut.Delete(existingId);

                Assert.IsTrue(assertContext.Notifications.Any(n => n.Id == existingId && n.IsDeleted == true));
            }
        }
    }
}
