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
		public Startup(IConfiguration configuration, IHostingEnvironment env)
		{
			this.Configuration = configuration;
			this.Environment = env;
		}

		public IConfiguration Configuration { get; }
		public IHostingEnvironment Environment { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			//services.Configure<CookiePolicyOptions>(options =>
			//{
			//    options.CheckConsentNeeded = context => true;
			//    options.MinimumSameSitePolicy = SameSiteMode.None;
			//});
			var connectionString = System.Environment
								.GetEnvironmentVariable("SDConnectionString", EnvironmentVariableTarget.User);

			services.AddDbContext<SmartDormitoryContext>(options => options.UseSqlServer(connectionString));
			services.AddIdentity<User, IdentityRole>()
				.AddEntityFrameworkStores<SmartDormitoryContext>()
				.AddDefaultTokenProviders();

            // IMPORTANT
            // Comment this line if dropped db and update again
            GlobalConfiguration.Configuration.UseSqlServerStorage(connectionString);
			services.AddHangfire(config => config.UseSqlServerStorage(connectionString));

			// Dependency Injection
			services.AddHttpClient<IcbHttpClient>();
			services.AddScoped<UserManager<User>>();
			services.AddScoped<RoleManager<IdentityRole>>();

			services.AddScoped<IUserService, UserService>();
			services.AddTransient<IIcbApiService, IcbApiService>();
			services.AddTransient<IIcbSensorsService, IcbSensorsService>();
			services.AddTransient<ISensorsService, SensorsService>();
			services.AddTransient<IMeasureTypeService, MeasureTypeService>();

			services.AddTransient<IHangfireJobsScheduler, HangfireJobsScheduler>();

			// Comment this line if database is dropped for the first start of the program
			this.ActivatingHangfireJobs(services);

			if (this.Environment.IsDevelopment())
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

			services.ConfigureApplicationCookie(options =>
			{
				options.LoginPath = $"/Identity/Account/Login";
				options.LogoutPath = $"/Identity/Account/Logout";
				options.AccessDeniedPath = $"/Identity/Account/AccessDenied";
			});

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

			//hangfire
			var hangfireServerOptions = new BackgroundJobServerOptions
			{
				SchedulePollingInterval = TimeSpan.FromSeconds(1)
			};

			app.UseHangfireDashboard();
			app.UseHangfireServer(hangfireServerOptions);

			app.UseMvc(routes =>
			{
				routes.MapRoute(
					name: "Administration",
					template: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

				routes.MapRoute(
					name: "default",
					template: "{controller=Home}/{action=Index}/{id?}");
			});
		}

		private void ActivatingHangfireJobs(IServiceCollection services)
		{
			var sp = services.BuildServiceProvider();
			var hangFireServices = sp.GetService<IHangfireJobsScheduler>();

			hangFireServices.StartingJobsQueue();
		}
	}
}
