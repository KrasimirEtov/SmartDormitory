using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SmartDormitory.App.Data;
using SmartDormitory.App.Infrastructure.Middleware;
using SmartDormitory.Data.Models;
using System.Threading.Tasks;

namespace SmartDormitory.App.Infrastructure.Extensions
{
	public static class ApplicationBuilderExtensions
	{

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

		public static void UseWrongRouteHandler(this IApplicationBuilder builder)
		{
			builder.UseMiddleware<WrongRouteMiddleware>();
		}
	}
}
