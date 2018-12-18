using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SmartDormitory.App.Data;
using SmartDormitory.Data.Models;
using SmartDormitory.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SmartDormitory.Tests.SmartDormitory.ServicesTests.NotificationsService.Tests
{
    [TestClass]
    public class DeleteAllHistory_Should
    {
        private DbContextOptions<SmartDormitoryContext> contextOptions;

        [TestMethod]
        public async Task DeleteAll_WhenSeenExisting()
        {
            // Arrange
            contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
           .UseInMemoryDatabase(databaseName: "DeleteAll_WhenSeenExisting")
               .Options;

            using (var assertContext = new SmartDormitoryContext(contextOptions))
            {
                await assertContext.Notifications.AddRangeAsync(new Notification
                {
                    Id = Guid.NewGuid().ToString(),
                    Message = "Some message",
                    Title = "Some title",
                    IsDeleted = false,
                    Seen = true
                },
                new Notification
                {
                    Id = Guid.NewGuid().ToString(),
                    Message = "Some message",
                    Title = "Some title",
                    IsDeleted = false,
                    Seen = true
                });
                await assertContext.SaveChangesAsync();
            }

            // Act && Asert
            using (var assertContext = new SmartDormitoryContext(contextOptions))
            {
                var sut = new NotificationService(assertContext);
                await sut.DeleteAllHistory();

                Assert.IsFalse(assertContext.Notifications.Any(n => n.IsDeleted == false));
            }
        }
    }
}
