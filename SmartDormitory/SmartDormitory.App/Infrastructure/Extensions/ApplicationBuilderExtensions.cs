using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SmartDormitory.App.Data;
using SmartDormitory.Data.Models;
using System.Threading.Tasks;

namespace SmartDormitory.App.Infrastructure.Extensions
{
	public static class ApplicationBuilderExtensions
	{
		public static IApplicationBuilder UseDatabaseMigration(this IApplicationBuilder app)
		{
			using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
			{
				serviceScope.ServiceProvider.GetService<SmartDormitoryContext>().Database.Migrate();

				//    // Seeding the default administrator
				//    var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>();
				//    var roleManager = serviceScope.ServiceProvider.GetService<RoleManager<IdentityRole>>();

				//    var adminName = "Administrator";
				//    var regularName = "RegularUser";
				//    var moderatorRole = "Moderator";

				//    // This allows us to call asynchronous code in a synchronous context and await it
				//    Task.Run(async () =>
				//    {
				//        if (await roleManager.RoleExistsAsync(regularName) == false)
				//        {
				//            await roleManager.CreateAsync(new IdentityRole
				//            {
				//                Name = regularName
				//            });
				//        }

				//        if (await roleManager.RoleExistsAsync(moderatorRole) == false)
				//        {
				//            await roleManager.CreateAsync(new IdentityRole
				//            {
				//                Name = moderatorRole
				//            });
				//        }

				//        var roleExists = await roleManager.RoleExistsAsync(adminName);

				//        if (!roleExists)
				//        {
				//            await roleManager.CreateAsync(new IdentityRole
				//            {
				//                Name = adminName
				//            });
				//        }

				//        var adminEmail = "a@a.a";
				//        var adminUser = await userManager.FindByNameAsync(adminEmail);

				//        if (adminUser == null)
				//        {
				//            adminUser = new User
				//            {
				//                Email = adminEmail,
				//                UserName = adminEmail,
				//            };

				//            await userManager.CreateAsync(adminUser, "admin");

				//            await userManager.AddToRoleAsync(adminUser, adminName);
				//        }
				//    })
				//    .Wait();

				return app;
			}
		}

		public static IApplicationBuilder SeedAdminAccount(this IApplicationBuilder app)
		{
			// TODO: Move this data to enviroment variable both on our machines and to azure
			const string adminRoleName = "Administrator";
			string adminEmail = "admin@admin";
			string adminUserName = "admin";
			string adminPassword = "admin";
			using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
			{
				var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

				Task<bool> roleExists = roleManager.RoleExistsAsync(adminRoleName);
				roleExists.Wait();

				if (!roleExists.Result)
				{
					Task<IdentityResult> roleResult = roleManager.CreateAsync(new IdentityRole(adminRoleName));
					roleResult.Wait();
				}

				var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<User>>();
				Task.Run(async () =>
				 {
					 var adminUser = await userManager.FindByEmailAsync(adminEmail);
					 if (adminUser == null)
					 {
						 adminUser = new User
						 {
							 Email = adminEmail,
							 UserName = adminUserName,
						 };
						 await userManager.CreateAsync(adminUser, adminPassword);
						 await userManager.AddToRoleAsync(adminUser, adminRoleName);
					 }				 
				 })
				 .Wait();
			}
			return app;
		}
	}
}
