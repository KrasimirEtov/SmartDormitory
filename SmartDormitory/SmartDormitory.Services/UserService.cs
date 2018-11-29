using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SmartDormitory.App.Data;
using SmartDormitory.Data.Models;
using SmartDormitory.Services.Abstract;
using SmartDormitory.Services.Contracts;
using SmartDormitory.Services.Exceptions;
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

        public async Task<IEnumerable<User>> GetAllUsers()
        {
            return await this.Context.Users
                .Include(s => s.Sensors)
                .ToListAsync();
        }

        public async Task<bool> IsInRole(string userId, string roleName)
        {
            var user = await GetUser(userId);

			if (user == null)
			{
				throw new EntityDoesntExistException("User does not exist");
			}

            return await this.userManager
                .IsInRoleAsync(user, roleName);
        }

        public async Task SetRole(string userId, string roleName)
        {
            var user = await GetUser(userId);

			if (user == null)
			{
				throw new EntityDoesntExistException("User does not exist");
			}

            await this.userManager
				.AddToRoleAsync(user, roleName);
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
            this.Context.Users.Remove(user);
            await this.Context.SaveChangesAsync();
        }
    }
}
