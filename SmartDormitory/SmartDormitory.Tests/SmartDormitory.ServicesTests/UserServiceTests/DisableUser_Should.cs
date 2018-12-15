using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SmartDormitory.App.Data;
using SmartDormitory.Data.Models;
using SmartDormitory.Services;
using SmartDormitory.Services.Exceptions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SmartDormitory.Tests.SmartDormitory.ServicesTests.UserServiceTests
{
	[TestClass]
	public class DisableUser_Should
	{
		private DbContextOptions<SmartDormitoryContext> contextOptions;
		private Mock<UserManager<User>> userManagerMock;
		private Mock<RoleManager<IdentityRole>> roleManagerMock;
		private User user;

		[TestMethod]
		public async Task Throw_EntityDoesntExistException_When_User_Does_Not_Exist()
		{
			// Arrange
			contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
			.UseInMemoryDatabase(databaseName: "DisableUser_Throw_EntityDoesntExistException_When_User_Does_Not_Exist")
				.Options;

			string userId = Guid.NewGuid().ToString();

			userManagerMock = MockUserManager<User>();

			roleManagerMock = MockRoleManager();

			// Act && Assert
			using (var assertContext = new SmartDormitoryContext(contextOptions))
			{
				var userService = new UserService(assertContext, userManagerMock.Object, roleManagerMock.Object);

				await Assert.ThrowsExceptionAsync<EntityDoesntExistException>(
					() => userService.DisableUser(userId));
			}
		}

		[TestMethod]
		public async Task Set_IsLocked_To_False_When_User_IsLocked()
		{
			// Arrange
			contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
			.UseInMemoryDatabase(databaseName: "Set_IsLocked_To_False_When_User_IsLocked")
				.Options;

			string userId = Guid.NewGuid().ToString();

			user = new User()
			{
				Id = userId,
				UserName = "testUserName",
				IsLocked = true
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
				await userService.DisableUser(userId);
				userManagerMock.Verify(x => x.SetLockoutEnabledAsync(It.IsAny<User>(), It.IsAny<bool>()), Times.Once);
				user = await assertContext.Users
					.Where(u => u.Id == userId).FirstOrDefaultAsync();
				Assert.IsFalse(user.IsLocked);
			}
		}

		[TestMethod]
		public async Task Set_IsLocked_To_True_When_User_Is_Not_Locked()
		{
			// Arrange
			contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
			.UseInMemoryDatabase(databaseName: "Set_IsLocked_To_True_When_User_Is_Not_Locked")
				.Options;

			string userId = Guid.NewGuid().ToString();

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
				await userService.DisableUser(userId);
				user = await assertContext.Users
					.Where(u => u.Id == userId).FirstOrDefaultAsync();
				Assert.IsTrue(user.IsLocked);
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
