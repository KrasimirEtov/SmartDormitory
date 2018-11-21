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
			// TODO: Delete user account
			var users = await userService.GetAllUsers();
			var userViewModels = users.Select(u => new UserViewModel(u)).ToList();

			foreach (var user in userViewModels)
			{
				if (await userService.IsInRole(user.Id, "Administrator"))
				{
					user.IsAdmin = true;
				}
			}
			var userViewModelList = new UserViewModelList(userViewModels);

			return View(userViewModelList);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> ToggleRole(string userId)
		{
			var user = await this.userService.GetUser(userId);
			if (user == null)
			{
				this.TempData["Error-Message"] = $"User does not exist!";
				return NoContent();
			}

			if (await userService.IsInRole(userId, "Administrator"))
			{
				this.TempData["Success-Message"] = $"{user.UserName} successfully removed!";

				await this.userService.RemoveRole(user.Id, "Administrator");
			}
			else
			{
				this.TempData["Success-Message"] = $"You successfully made [{user.UserName}] administrator!";

				await this.userService.SetRole(user.Id, "Administrator");
			}

			return NoContent();
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Delete(string userId)
		{
			var user = await this.userService.GetUser(userId);
			if (user == null)
			{
				this.TempData["Error-Message"] = $"User does not exist!";
				return NoContent();
			}

			await this.userService.DeleteUser(userId);

			this.TempData["Success-Message"] = $"{user.UserName} successfully removed!";

			return NoContent(); 
		}
	}
}
