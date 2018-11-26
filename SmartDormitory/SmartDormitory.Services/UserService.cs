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
            return await this.userManager
                .IsInRoleAsync(user, roleName);
        }

        public async Task CreateRole(string roleName)
        {
            var roleExists = await this.roleManager.RoleExistsAsync(roleName);
            if (roleExists)
            {
                throw new EntityAlreadyExistsException("Role already exists");
            }
            await this.roleManager.CreateAsync(new IdentityRole(roleName));
        }

        public async Task SetRole(string userId, string roleName)
        {
            var user = await GetUser(userId);
            await this.userManager.AddToRoleAsync(user, roleName);
        }

        public async Task RemoveRole(string userId, string roleName)
        {
            var user = await GetUser(userId);
            await this.userManager.RemoveFromRoleAsync(user, roleName);
        }

        public async Task DeleteUser(string userId)
        {
            var user = await GetUser(userId);
            if (user == null || user.IsDeleted)
            {
                throw new EntityDoesntExistException($"\nUser doesn't exists!");
            }

            this.Context.Users.Remove(user);
            await this.Context.SaveChangesAsync();
        }
    }
}
