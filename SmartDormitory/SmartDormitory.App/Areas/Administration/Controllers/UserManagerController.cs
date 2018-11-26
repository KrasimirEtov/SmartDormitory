using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartDormitory.App.Areas.Administration.Models;
using SmartDormitory.Services.Contracts;
using System.Linq;
using System.Threading.Tasks;

namespace SmartDormitory.App.Areas.Administration.Controllers
{
	[Area("Administration")]
	[Authorize(Roles = "Administrator")]
	public class UserManagerController : Controller
	{
		private readonly IUserService userService;

		public UserManagerController(IUserService userService)
		{
			this.userService = userService;
		}

		[HttpGet]
		public async Task<IActionResult> Users()
		{
			// TODO: Return a partial view for pagination
			var users = await userService.GetAllUsers();
			var userViewModels = users.Select(u => new UserViewModel(u)).ToList();

			foreach (var user in userViewModels)
			{
				if (await userService.IsInRole(user.Id, "Administrator"))
				{
					user.IsAdmin = true;
				}
			}

			return View(userViewModels);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ToggleRole([FromForm]string userId)
		{
			
			var user = await this.userService.GetUser(userId);
			if (user == null)
			{
				this.TempData["Error-Message"] = $"User does not exist!";
				return this.NotFound();
			}

			if (await userService.IsInRole(userId, "Administrator"))
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
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete([FromForm]string userId)
		{
			var user = await this.userService.GetUser(userId);
			if (user == null)
			{
				this.TempData["Error-Message"] = $"User does not exist!";
				return this.NotFound();
			}

			await this.userService.DeleteUser(userId);

			this.TempData["Success-Message"] = $"{user.UserName} successfully removed!";

			return this.Ok();
		}
	}
}
