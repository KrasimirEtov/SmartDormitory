using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SmartDormitory.App.Data;
using SmartDormitory.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartDormitory.Tests.SmartDormitory.ServicesTests.NotificationsService.Tests
{
    [TestClass]
    public class GetAllByUserId_Should
    {
        private object contextOptions;

        [TestMethod]
        public async Task ThrowArugmentNullException_When_PassedNullUserId()
        {
            // Arrange
            var contextMock = new Mock<SmartDormitoryContext>();

            var sut = new NotificationService(contextMock.Object);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentNullException>(
                () => sut.GetAllByUserId(null), "Parameter userId cannot be null!");
        }

        [TestMethod]
        public async Task Throw_ArugmentException_When_PassedInvalidUserIdGuid()
        {
            // Arrange
            var contextMock = new Mock<SmartDormitoryContext>();

            var sut = new NotificationService(contextMock.Object);

            // Act & Assert
            await Assert.ThrowsExceptionAsync<ArgumentException>(
                () => sut.GetAllByUserId("InvalidGuid"));
        }
    }
}