
using SmartDormitory.Data.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SmartDormitory.Services.Contracts
{
	public interface IUserService
	{
		Task<IEnumerable<User>> GetAllUsers();
		Task<User> GetUser(string userId);
		Task<bool> IsInRole(string userId, string roleName);
		Task CreateRole(string roleName);
		Task SetRole(string userId, string roleName);
		Task RemoveRole(string userId, string roleName);
		Task DeleteUser(string userId);
	}
}
