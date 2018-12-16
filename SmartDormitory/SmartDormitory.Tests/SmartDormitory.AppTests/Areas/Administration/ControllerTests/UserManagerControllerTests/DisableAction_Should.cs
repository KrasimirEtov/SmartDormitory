using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SmartDormitory.App.Areas.Administration.Controllers;
using SmartDormitory.Data.Models;
using SmartDormitory.Services.Contracts;
using SmartDormitory.Services.Exceptions;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SmartDormitory.Tests.SmartDormitory.AppTests.Areas.Administration.ControllerTests.UserManagerControllerTests
{
	[TestClass]
	public class DisableAction_Should
	{
		private Mock<IUserService> userServiceMock = new Mock<IUserService>();
		private UserManagerController controller;

		[TestMethod]
		public async Task ToggleRoleAction_Returns_OkResult_When_No_Exception_Is_Thrown()
		{
			// Arrange && Act
			var controller = SetupController(1);

			var result = await controller.Disable("userId");

			// Assert
			Assert.IsInstanceOfType(result, typeof(OkResult));
			userServiceMock.Verify(a => a.DisableUser(It.IsAny<string>()), Times.Once);
		}

		[TestMethod]
		public async Task IndexAction_ReturnsToIndexUserManager_WhenUserIsNull_RedirectResult()
		{
			// Arrange
			var controller = this.SetupController(2);

			// Act
			var result = await controller.Disable("userId");

			// Assert
			Assert.IsInstanceOfType(result, typeof(NotFoundObjectResult));
		}

		private Mock<IUserService> SetupMockService(int test)
		{
			switch (test)
			{
				case 1:
					// When everything is okay
					break;

				case 2:
					userServiceMock
						.Setup(x => x.DisableUser(It.IsAny<string>()))
						.ThrowsAsync(new EntityDoesntExistException("test"));				
					break;
			}
			return userServiceMock;
		}

		private UserManagerController SetupController(int test)
		{
			switch (test)
			{
				case 1:
					// user list and is admin true
					userServiceMock = SetupMockService(test);
					break;
				case 2:
					// user == null
					userServiceMock = SetupMockService(test);
					break;
			}

			controller = new UserManagerController(userServiceMock.Object)
			{
				ControllerContext = new ControllerContext()
				{
					HttpContext = new DefaultHttpContext()
					{
						User = new ClaimsPrincipal()
					}
				},
				TempData = new Mock<ITempDataDictionary>().Object
			};

			return controller;
		}
	}
}
