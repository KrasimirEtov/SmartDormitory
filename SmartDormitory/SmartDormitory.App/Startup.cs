using Hangfire;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SmartDormitory.App.Data;
using SmartDormitory.App.Infrastructure.Extensions;
using SmartDormitory.App.Infrastructure.Filters;
using SmartDormitory.App.Infrastructure.Hangfire;
using SmartDormitory.Data.Models;
using SmartDormitory.Services;
using SmartDormitory.Services.Contracts;
using SmartDormitory.Services.HttpClients;
using System;

namespace SmartDormitory.App
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IHostingEnvironment environment)
        {
            this.Configuration = configuration;
            this.HostingEnvironment = environment;
        }

        public IConfiguration Configuration { get; }
        public IHostingEnvironment HostingEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            this.RegisterData(services);
            this.RegisterAuthentication(services);
            //this.RegisterAuthorizations(services);
            this.RegisterServices(services);
            this.RegisterInfrastructure(services);

            // IMPORTANT
            // Comment this line if dropped db and update-database       
            this.RegisterHangfireDbTables(services);
        }

        //todo use later?
        private void RegisterAuthorizations(IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("Admin", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireRole("Administrator");
                });
            });
        }

        private void RegisterHangfireDbTables(IServiceCollection services)
        {
            var connectionString = Environment
                                            .GetEnvironmentVariable("SDConnectionString", EnvironmentVariableTarget.User);

            GlobalConfiguration.Configuration.UseSqlServerStorage(connectionString);
            services.AddHangfire(config => config.UseSqlServerStorage(connectionString));
        }

        private void RegisterServices(IServiceCollection services)
        {
            services.AddHttpClient<IcbHttpClient>();
            services.AddScoped<UserManager<User>>();
            services.AddScoped<RoleManager<IdentityRole>>();

            services.AddScoped<IUserService, UserService>();
            services.AddTransient<IIcbApiService, IcbApiService>();
            services.AddTransient<IIcbSensorsService, IcbSensorsService>();
            services.AddTransient<ISensorsService, SensorsService>();
            services.AddTransient<IMeasureTypeService, MeasureTypeService>();
            services.AddTransient<INotificationService, NotificationService>();

            services.AddTransient<IHangfireJobsScheduler, HangfireJobsScheduler>();
        }

        private void RegisterAuthentication(IServiceCollection services)
        {
            services.AddIdentity<User, IdentityRole>()
                    .AddEntityFrameworkStores<SmartDormitoryContext>()
                    .AddDefaultTokenProviders();

            if (this.HostingEnvironment.IsDevelopment())
            {
                services.Configure<IdentityOptions>(options =>
                {
                    // Password settings
                    options.Password.RequireDigit = false;
                    options.Password.RequiredLength = 3;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequiredUniqueChars = 0;

                    // Lockout settings
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(1);
                    options.Lockout.MaxFailedAccessAttempts = 999;
                });
            }
        }

        private void RegisterInfrastructure(IServiceCollection services)
        {
            //services.AddMemoryCache();

            services
              .AddMvc(options =>
              {
                  options.Filters.Add<AutoValidateAntiforgeryTokenAttribute>();
              })
              .SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
              .AddRazorPagesOptions(options =>
              {
                  options.AllowAreas = true;
                  options.Conventions.AuthorizeAreaFolder("Identity", "/Account/Manage");
                  options.Conventions.AuthorizeAreaPage("Identity", "/Account/Logout");
              });
        }

        private void RegisterData(IServiceCollection services)
        {
            string connectionString = string.Empty;
            if (HostingEnvironment.IsDevelopment())
            {
                connectionString = Environment
                                    .GetEnvironmentVariable("SDConnectionString", EnvironmentVariableTarget.User);
            }
            else
            {
                //TODO: add azure connection string
                //connectionString = azure connection string
            }

            services.AddDbContext<SmartDormitoryContext>(options => options.UseSqlServer(connectionString));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseAuthentication();
            app.SeedAdminAccount();

            var hangfireServerOptions = new BackgroundJobServerOptions
            {
                SchedulePollingInterval = TimeSpan.FromSeconds(5),
                Queues = new[] { "sensordata", "default" }
            };

            //TODO add app extension method
            var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetService<SmartDormitoryContext>();
            context.Database.Migrate();

            //make dashboard visible only for Admins
            app.UseHangfireDashboard("/hangfire", new DashboardOptions
            {
                Authorization = new[] { new HangfireAuthorizeFilter() } 
            });
            app.UseHangfireServer(hangfireServerOptions);

            RecurringJob.AddOrUpdate<IIcbSensorsService>(job => job.AddSensorsAsync(), Cron.Hourly());
            //IMPORTANT
            //comment after 1st start in dev or u will have +1 of the same job
            var jobId = BackgroundJob.Enqueue<IHangfireJobsScheduler>(s => s.UpdateSensorsData());
            //BackgroundJob.Enqueue<IHangfireJobsScheduler>(s => s.StartingJobsQueue());
            
            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "areas",
                    template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        //private void ActivatingHangfireJobs(IServiceCollection services)
        //{
        //    var sp = services.BuildServiceProvider();
        //    var hangFireServices = sp.GetService<IHangfireJobsScheduler>();

        //    hangFireServices.StartingJobsQueue();
        //}
    }
}
