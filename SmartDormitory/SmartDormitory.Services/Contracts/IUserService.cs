﻿
using SmartDormitory.Data.Models;
using SmartDormitory.Services.Models.Users;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartDormitory.Services.Contracts
{
	public interface IUserService
	{
		Task<IEnumerable<UserListServiceModel>> GetAllUsers(int page = 1, int pageSize = 4);
		Task<User> GetUser(string userId);
		Task<bool> IsAdmin(string userId);
		Task SetRole(string userId, string roleName);
		Task RemoveRole(string userId, string roleName);
		Task DisableUser(string userId);
		Task<int> TotalUsers();
		Task SetGdprStatus(string userId);
		Task DeleteUser(string userId);
	}
}
