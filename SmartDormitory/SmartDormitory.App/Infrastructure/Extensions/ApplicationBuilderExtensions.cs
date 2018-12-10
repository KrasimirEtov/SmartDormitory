using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SmartDormitory.App.Data;
using SmartDormitory.App.Infrastructure.Middleware;
using SmartDormitory.Data.Models;
using System;
using System.Collections.Generic;
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
                             AgreedGDPR = true
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

        public static IApplicationBuilder SeedMeasureTypes(this IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var dbContext = serviceScope
                                    .ServiceProvider
                                    .GetRequiredService<SmartDormitoryContext>();

                if (!dbContext.MeasureTypes.AnyAsync().GetAwaiter().GetResult())
                {
                    var addList = new List<MeasureType>
                    {
                        new MeasureType
                        {
                            Id = Guid.NewGuid().ToString(),
                            MeasureUnit = "°C",
                            SuitableSensorType = "Temperature",
                            CreatedOn = DateTime.Now
                        },
                        new MeasureType
                        {
                            Id = Guid.NewGuid().ToString(),
                            MeasureUnit = "%",
                            SuitableSensorType = "Humidity",
                            CreatedOn = DateTime.Now
                        },
                        new MeasureType
                        {
                            Id = Guid.NewGuid().ToString(),
                            MeasureUnit = "W",
                            SuitableSensorType = "Electric power consumtion",
                            CreatedOn = DateTime.Now
                        },
                        new MeasureType
                        {
                            Id = Guid.NewGuid().ToString(),
                            MeasureUnit = "(true/false)",
                            SuitableSensorType = "Boolean switch (door/occupancy/etc)",
                            CreatedOn = DateTime.Now
                        },
                        new MeasureType
                        {
                            Id = Guid.NewGuid().ToString(),
                            MeasureUnit = "dB",
                            SuitableSensorType = "Noise",
                            CreatedOn = DateTime.Now
                        }
                    };

                    dbContext.AddRangeAsync(addList).GetAwaiter().GetResult();
                    dbContext.SaveChangesAsync().GetAwaiter().GetResult();
                }
            }

            return app;
        }
    }
}
