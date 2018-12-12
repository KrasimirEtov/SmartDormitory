using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SmartDormitory.Data.Models;
using SmartDormitory.Services.Contracts;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;

namespace SmartDormitory.App.Areas.Identity.Pages.Account
{
	[AllowAnonymous]
	public class RegisterModel : PageModel
	{
		private readonly SignInManager<User> _signInManager;
		private readonly IUserService userService;
		private readonly UserManager<User> _userManager;

		public RegisterModel(
			UserManager<User> userManager,
			SignInManager<User> signInManager,
			IUserService userService)
		{
			_userManager = userManager;
			_signInManager = signInManager;
			this.userService = userService;
		}

		[BindProperty]
		public InputModel Input { get; set; }

		public string ReturnUrl { get; set; }

		public class InputModel
		{
			[Required]
			[StringLength(25, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
			[Display(Name = "Username")]
			public string Username { get; set; }

			[Required]
			[EmailAddress]
			[Display(Name = "Email")]
			public string Email { get; set; }

			//TODO change min lentgh
			[Required]
			[StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
			[DataType(DataType.Password)]
			[Display(Name = "Password")]
			public string Password { get; set; }

			[DataType(DataType.Password)]
			[Display(Name = "Confirm password")]
			[Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
			public string ConfirmPassword { get; set; }

			[Required]
			[Display(Name = "GDPR template")]
			public bool AgreedGDPR { get; set; }
		}

		public void OnGet(string returnUrl = null)
		{
			ReturnUrl = returnUrl;
		}

		public async Task<IActionResult> OnPostAsync(string returnUrl = null)
		{
			returnUrl = returnUrl ?? Url.Content("~/");
			if (!Input.AgreedGDPR)
			{
				TempData["Error-Message"] = "You need to agree with out GDPR policy.";
				return Page();
			}
			if (ModelState.IsValid)
			{
				var user = new User { UserName = Input.Username, Email = Input.Email };
				var result = await _userManager.CreateAsync(user, Input.Password);
				if (result.Succeeded)
				{
					await userService.SetGdprStatus(user.Id);
					await _signInManager.SignInAsync(user, isPersistent: false);
					return LocalRedirect(returnUrl);
				}
				foreach (var error in result.Errors)
				{
					ModelState.AddModelError(string.Empty, error.Description);
				}
			}
			return Page();
		}
	}
}
