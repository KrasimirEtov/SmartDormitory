using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartDormitory.App.Data;
using SmartDormitory.Data.Models;
using SmartDormitory.Services.Abstract;
using SmartDormitory.Services.Contracts;
using SmartDormitory.Services.Exceptions;
using SmartDormitory.Services.Models.Users;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartDormitory.Services
{
	public class UserService : BaseService, IUserService
	{
		private readonly UserManager<User> userManager;
		private readonly RoleManager<IdentityRole> roleManager;

		public UserService(SmartDormitoryContext context,
			UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
			: base(context)
		{
			this.userManager = userManager;
			this.roleManager = roleManager;
		}

		public async Task<User> GetUser(string userId)
		{
			return await this.Context.Users
				.Where(u => u.Id == userId)

				.FirstOrDefaultAsync();
		}

		public async Task<IEnumerable<UserListServiceModel>> GetAllUsers(int page = 1, int pageSize = 3)
		{
			return await this.Context.Users
				.OrderByDescending(u => u.CreatedOn)
				.Skip((page - 1) * pageSize)
				.Take(pageSize)
				.Where(u => u.IsDeleted == false)
				.Select(u => new UserListServiceModel
				{
					FirstName = u.FirstName,
					LastName = u.LastName,
					Id = u.Id,
					IsDeleted = u.IsDeleted,
					UserName = u.UserName,
					SensorsCount = u.Sensors.Count // check if it works, otherwise include sensors
				})
				.ToListAsync();
		}

		public async Task<bool> IsAdmin(string userId)
		{
			var user = await GetUser(userId);
			if (user == null)
			{
				throw new EntityDoesntExistException("User does not exist");
			}
			return await this.userManager.IsInRoleAsync(user, "Administrator");
		}

		public async Task SetRole(string userId, string roleName)
		{
			var user = await GetUser(userId);
			if (user == null)
			{
				throw new EntityDoesntExistException("User does not exist");
			}
			await this.userManager.AddToRoleAsync(user, roleName);
		}

		public async Task RemoveRole(string userId, string roleName)
		{
			var user = await GetUser(userId);
			if (user == null)
			{
				throw new EntityDoesntExistException("User does not exist");
			}
			await this.userManager.RemoveFromRoleAsync(user, roleName);
		}

		public async Task DeleteUser(string userId)
		{
			var user = await GetUser(userId);
			if (user == null || user.IsDeleted)
			{
				throw new EntityDoesntExistException($"\nUser doesn't exists!");
			}

			// TODO: This does not set IsDeleted flag, it directly deletes the entity
			// TODO: user.IsDeleted = true;
			this.Context.Users.Remove(user);
			await this.Context.SaveChangesAsync();
		}

		public async Task<int> TotalUsers()
			=> await this.Context.Users.CountAsync(u => u.IsDeleted == false);
	}
}
