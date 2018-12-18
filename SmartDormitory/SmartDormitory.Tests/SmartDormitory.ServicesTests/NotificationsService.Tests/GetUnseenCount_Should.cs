using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SmartDormitory.App.Data;
using SmartDormitory.Data.Models;
using SmartDormitory.Services;
using System;
using System.Threading.Tasks;

namespace SmartDormitory.Tests.SmartDormitory.ServicesTests.NotificationsService.Tests
{
    [TestClass]
    public class GetUnseenCount_Should
    {
        private DbContextOptions<SmartDormitoryContext> contextOptions;

        [TestMethod]
        public async Task ThrowArugmentNullException_WhenPassedNullUserId()
        {
            // Arrange
            var contextMock = new Mock<SmartDormitoryContext>();

            var sut = new NotificationService(contextMock.Object);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(
                () => sut.GetUnseenCount(null), "Parameter userId cannot be null!");
        }

        [TestMethod]
        public async Task Throw_ArugmentException_When_Passed_Invalid_Guid()
        {
            // Arrange
            var contextMock = new Mock<SmartDormitoryContext>();

            var sut = new NotificationService(contextMock.Object);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => sut.GetUnseenCount("InvalidGuid"));
        }

        [TestMethod]
        public async Task ReturnCorrectCount_WhenPassedValidId()
        {
            // Arrange
            contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
           .UseInMemoryDatabase(databaseName: "ReturnCorrectCount_WhenPassedValidId")
               .Options;

            var existingId = Guid.NewGuid().ToString();
            using (var assertContext = new SmartDormitoryContext(contextOptions))
            {
                await assertContext.Notifications.AddRangeAsync(new Notification
                {
                    Id = Guid.NewGuid().ToString(),
                    Message = "Some message",
                    Title = "Some title",
                    IsDeleted = false,
                    ReceiverId = existingId
                });
                await assertContext.SaveChangesAsync();
            }

            // Act && Asert
            using (var assertContext = new SmartDormitoryContext(contextOptions))
            {
                var sut = new NotificationService(assertContext);
                var result = await sut.GetUnseenCount(existingId);

                Assert.IsTrue(result == 1);
            }
        }
    }
}