using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using SmartDormitory.App.Data;
using SmartDormitory.Data.Models;
using SmartDormitory.Services;
using System.Linq;
using System.Threading.Tasks;

namespace SmartDormitory.Tests.SmartDormitory.AppTests.UserServiceTests
{
	[TestClass]
	public class GetAllUsers_Should
	{
		private DbContextOptions<SmartDormitoryContext> contextOptions;
		private Mock<UserManager<User>> userManagerMock;
		private Mock<RoleManager<IdentityRole>> roleManagerMock;
		private User user;

		[TestMethod]
		public async Task Return_Valid_Users_List()
		{
			// Arrange
			contextOptions = new DbContextOptionsBuilder<SmartDormitoryContext>()
			.UseInMemoryDatabase(databaseName: "Return_Valid_Users_List")
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

				var users = await userService.GetAllUsers();

				Assert.IsTrue(users.ToList().Count == 1);
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
