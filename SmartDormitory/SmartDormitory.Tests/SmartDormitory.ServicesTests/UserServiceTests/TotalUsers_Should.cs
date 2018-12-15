using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SmartDormitory.App.Data;
using SmartDormitory.Data.Models;
using SmartDormitory.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace SmartDormitory.Tests.SmartDormitory.ServicesTests.UserServiceTests
{
	[TestClass]
	public class TotalUsers_Should
	{
		private DbContextOptions<SmartDormitoryContext> contextOptions;
		private Mock<UserManager<User>> userManagerMock;
		private Mock<RoleManager<IdentityRole>> roleManagerMock;
		private User user;

		[TestMethod]
		public async Task Return_Valid_Users_Count()
		{
			// Arrange
			contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
			.UseInMemoryDatabase(databaseName: "Return_Valid_Users_Count")
				.Options;

			string userId = "myId";

			user = new User()
			{
				Id = userId,
				UserName = "testUserName"
			};

			userManagerMock = MockUserManager<User>();
			roleManagerMock = MockRoleManager();
			using (var actContext = new SmartDormitoryContext(contextOptions))
			{
				await actContext.Users.AddAsync(user);
				await actContext.SaveChangesAsync();
			}

			// Act && Assert
			using (var assertContext = new SmartDormitoryContext(contextOptions))
			{
				var userService = new UserService(assertContext, userManagerMock.Object, roleManagerMock.Object);

				var usersCount = await userService.TotalUsers();

				Assert.AreEqual(1, usersCount);
			}
		}

		private Mock<UserManager<TUser>> MockUserManager<TUser>() where TUser : class
		{
			var store = new Mock<IUserStore<TUser>>();
			var mgr = new Mock<UserManager<TUser>>(store.Object, null, null, null, null, null, null, null, null);
			mgr.Object.UserValidators.Add(new UserValidator<TUser>());
			mgr.Object.PasswordValidators.Add(new PasswordValidator<TUser>());

			return mgr;
		}

		private Mock<RoleManager<IdentityRole>> MockRoleManager()
		{
			var mockRoleManager = new Mock<RoleManager<IdentityRole>>(
				new Mock<IRoleStore<IdentityRole>>().Object,
				new IRoleValidator<IdentityRole>[0],
				new Mock<ILookupNormalizer>().Object,
				new Mock<IdentityErrorDescriber>().Object,
				new Mock<ILogger<RoleManager<IdentityRole>>>().Object);

			return mockRoleManager;
		}
	}
}
