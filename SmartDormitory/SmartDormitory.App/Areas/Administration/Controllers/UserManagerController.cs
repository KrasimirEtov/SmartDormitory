using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartDormitory.App.Areas.Administration.Models;
using SmartDormitory.App.Areas.Administration.Models.UserManager;
using SmartDormitory.Services.Contracts;
using SmartDormitory.Services.Exceptions;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace SmartDormitory.App.Areas.Administration.Controllers
{
	[Area("Administration")]
	[Authorize(Roles = "Administrator")]
	public class UserManagerController : Controller
	{
		private const int PageSize = 4;
		private readonly IUserService userService;

		public UserManagerController(IUserService userService)
		{
			this.userService = userService;
		}

		[HttpGet]
		public async Task<IActionResult> Index()
		{
			var model = await UpdateAllUsersPage();

			return View(model);

		}

		[HttpGet]
		public async Task<IActionResult> Users(int page = 1)
		{
			var model = await UpdateAllUsersPage(page);

			return PartialView("_UsersTablePartial", model);
		}

		[HttpPost]
		public async Task<IActionResult> ToggleRole([FromForm]string userId)
		{

			var user = await this.userService.GetUser(userId);
			if (user == null)
			{
				this.TempData["Error-Message"] = $"User does not exist!";
				return this.NotFound();
			}

			if (await userService.IsAdmin(user.Id))
			{
				// try catch ? 
				await this.userService.RemoveRole(user.Id, "Administrator");
				this.TempData["Success-Message"] = $"{user.UserName} successfully removed!";
			}
			else
			{
				await this.userService.SetRole(user.Id, "Administrator");
				this.TempData["Success-Message"] = $"You successfully made [{user.UserName}] administrator!";
			}

			return this.Ok();
		}

		[HttpPost]
		public async Task<IActionResult> Delete([FromForm]string userId)
		{
			try
			{
				await this.userService.DeleteUser(userId);
			}
			catch (EntityDoesntExistException e)
			{
				this.TempData["Error-Message"] = e.Message;
				// TODO: Talk about exception handling
				return this.NotFound(e.Message);
			}
			this.TempData["Success-Message"] = $"User was successfully deleted!";

			return this.Ok();
		}

		private async Task<UsersPagingViewModel> UpdateAllUsersPage(int page = 1)
		{
			var users = await userService.GetAllUsers(page);
			var userViewModels = users.Select(u => new UserViewModel(u)).ToList();
			var totalUsers = await userService.TotalUsers();

			foreach (var user in userViewModels)
			{
				user.IsAdmin = await userService.IsAdmin(user.Id);
			}

			var model = new UsersPagingViewModel
			{
				Users = userViewModels,
				CurrentPage = page,
				TotalPages = (int)Math.Ceiling(totalUsers / (double)PageSize)
			};

			return model;
		}
	}
}
