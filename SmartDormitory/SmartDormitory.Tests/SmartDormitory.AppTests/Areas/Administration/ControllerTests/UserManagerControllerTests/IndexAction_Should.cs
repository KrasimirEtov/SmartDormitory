using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SmartDormitory.App.Areas.Administration.Controllers;
using SmartDormitory.App.Areas.Administration.Models.UserManager;
using SmartDormitory.Data.Models;
using SmartDormitory.Services.Contracts;
using SmartDormitory.Services.Exceptions;
using SmartDormitory.Services.Models.Users;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SmartDormitory.Tests.SmartDormitory.AppTests.Areas.Administration.ControllerTests.UserManagerControllerTests
{
	[TestClass]
	public class IndexAction_Should
	{
		private Mock<IUserService> userServiceMock = new Mock<IUserService>();
		private User user;
		private UserManagerController controller;

		[TestMethod]
		public async Task IndexAction_Returns_ViewResult()
		{
			// Arrange && Act
			var controller = SetupController(1);

			var result = await controller.Index();

			// Assert
			Assert.IsInstanceOfType(result, typeof(ViewResult));
		}

		[TestMethod]
		public async Task IndexAction_Returns_Correct_ViewModel()
		{
			// Arrange && Act
			var controller = SetupController(1);

			var result = await controller.Index() as ViewResult;

			// Assert
			Assert.IsInstanceOfType(result.Model, typeof(UsersPagingViewModel));
		}

		[TestMethod]
		public async Task IndexAction_Calls_Correct_Service_Methods()
		{
			// Arrange && Act
			var controller = SetupController(1);

			var result = await controller.Index() as ViewResult;

			// Assert

			userServiceMock.Verify(s => s.GetAllUsers(It.IsAny<int>(), It.IsAny<int>()), Times.Once);
			userServiceMock.Verify(a => a.IsAdmin(It.IsAny<string>()), Times.AtLeastOnce);
		}

		[TestMethod]
		public async Task IndexAction_ReturnsToIndexUserManager_WhenUserIsNull_RedirectResult()
		{
			// Arrange
			var controller = this.SetupController(2);

			// Act
			var result = await controller.Index();

			// Assert
			Assert.IsInstanceOfType(result, typeof(RedirectToActionResult));
			var redirectResult = (RedirectToActionResult)result;
			Assert.AreEqual("Index", redirectResult.ActionName);
			Assert.AreEqual("UserManagerController", redirectResult.ControllerName);
			Assert.IsNull(redirectResult.RouteValues);
		}

		private Mock<IUserService> SetupMockService(int test)
		{
			var userListServiceModel = new UserListServiceModel()
			{
				FirstName = "firstName",
				LastName = "lastName",
				Id = Guid.NewGuid().ToString(),
				IsDeleted = false,
				UserName = "userName",
				SensorsCount = 1,
				IsLocked = false
			};

			var list = new List<UserListServiceModel>() { userListServiceModel };
			switch (test)
			{
				case 1:
					// When everything is okay

					userServiceMock
						.Setup(x => x.GetAllUsers(1, 4))
						.ReturnsAsync(list);

					userServiceMock
						.Setup(x => x.TotalUsers())
						.ReturnsAsync(1);

					userServiceMock
						.Setup(x => x.IsAdmin(It.IsAny<string>()))
						.ReturnsAsync(true);

					break;

				case 2:
					userServiceMock
						.Setup(x => x.GetAllUsers(1, 4))
						.ReturnsAsync(list);

					userServiceMock
						.Setup(x => x.TotalUsers())
						.ReturnsAsync(1);

					//userServiceMock
					//	.Setup(x => x.GetUser(It.IsAny<string>()))
					//	.ReturnsAsync(Task.FromResult<User>(null).Result);
					userServiceMock
						.Setup(x => x.IsAdmin(It.IsAny<string>()))
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
