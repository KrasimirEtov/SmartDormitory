using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SmartDormitory.App.Data;

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
    }
}
