using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SmartDormitory.App.Areas.Administration.Controllers;
using SmartDormitory.Data.Models;
using SmartDormitory.Services.Contracts;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace SmartDormitory.Tests.SmartDormitory.AppTests.Areas.Administration.ControllerTests.UserManagerControllerTests
{
	[TestClass]
	public class ToggleRoleAction_Should
	{
		private Mock<IUserService> userServiceMock = new Mock<IUserService>();
		private UserManagerController controller;

		[TestMethod]
		public async Task ToggleRoleAction_Returns_OkResult_When_User_Is_Admin()
		{
			// Arrange && Act
			var controller = SetupController(1);

			var result = await controller.ToggleRole("userId");

			// Assert
			Assert.IsInstanceOfType(result, typeof(OkResult));
			userServiceMock.Verify(a => a.IsAdmin(It.IsAny<string>()), Times.AtLeastOnce);
			userServiceMock.Verify(a => a.RemoveRole(It.IsAny<string>(), It.IsAny<string>()), Times.AtLeastOnce);
		}

		[TestMethod]
		public async Task ToggleRoleAction_Returns_NotFoundResult_When_User_Is_Null()
		{
			// Arrange && Act
			var controller = SetupController(2);

			var result = await controller.ToggleRole("userId");

			// Assert
			Assert.IsInstanceOfType(result, typeof(NotFoundResult));
		}

		[TestMethod]
		public async Task ToggleRoleAction_Returns_OkResult_When_User_Is_Not_Admin()
		{
			// Arrange && Act
			var controller = SetupController(3);

			var result = await controller.ToggleRole("userId");

			// Assert
			Assert.IsInstanceOfType(result, typeof(OkResult));
			userServiceMock.Verify(a => a.IsAdmin(It.IsAny<string>()), Times.AtLeastOnce);
			userServiceMock.Verify(a => a.SetRole(It.IsAny<string>(), It.IsAny<string>()), Times.AtLeastOnce);
		}

		private Mock<IUserService> SetupMockService(int test)
		{
			switch (test)
			{
				case 1:
					// When everything is okay
					userServiceMock
						.Setup(x => x.GetUser(It.IsAny<string>()))
						.ReturnsAsync(new User()
						{
							Id = Guid.NewGuid().ToString()
						});

					userServiceMock
						.Setup(x => x.IsAdmin(It.IsAny<string>()))
						.ReturnsAsync(true);
					break;

				case 2:
					userServiceMock
						.Setup(x => x.GetUser(It.IsAny<string>()))
						.ReturnsAsync(Task.FromResult<User>(null).Result);
					break;
				
				case 3:
					userServiceMock
						.Setup(x => x.GetUser(It.IsAny<string>()))
						.ReturnsAsync(new User()
						{
							Id = Guid.NewGuid().ToString()
						});

					userServiceMock
						.Setup(x => x.IsAdmin(It.IsAny<string>()))
						.ReturnsAsync(false);
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
				case 3:
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
