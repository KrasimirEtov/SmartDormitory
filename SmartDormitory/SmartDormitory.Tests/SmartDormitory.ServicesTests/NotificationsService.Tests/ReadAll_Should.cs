using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SmartDormitory.App.Data;
using SmartDormitory.Services;
using SmartDormitory.Services.Exceptions;
using System;
using System.Threading.Tasks;

namespace SmartDormitory.Tests.SmartDormitory.ServicesTests.NotificationsService.Tests
{
    [TestClass]
    public class ReadAll_Should
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
                () => sut.ReadAll(null), "Parameter userId cannot be null!");
        }

        [TestMethod]
        public async Task Throw_ArugmentException_When_Passed_Invalid_Guid()
        {
            // Arrange
            var contextMock = new Mock<SmartDormitoryContext>();

            var sut = new NotificationService(contextMock.Object);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => sut.ReadAll("InvalidGuid"));
        }

        [TestMethod]
        public async Task Throw_EntityDoesntExistException_When_UserDoesntExists()
        {
            // Arrange
            contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
        .UseInMemoryDatabase(databaseName: "Throw_EntityDoesntExistException_When_UserDoesntExists")
            .Options;

            // Act && Asert
            using (var assertContext = new SmartDormitoryContext(contextOptions))
            {
                var sut = new NotificationService(assertContext);

                await Assert.ThrowsExceptionAsync<EntityDoesntExistException>(
                    () => sut.ReadAll(Guid.NewGuid().ToString()));
            }
        }
    }
}

