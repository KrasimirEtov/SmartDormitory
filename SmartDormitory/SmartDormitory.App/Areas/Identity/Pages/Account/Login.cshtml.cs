using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using SmartDormitory.Data.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmartDormitory.App.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        private readonly SignInManager<User> signInManager;
        private readonly UserManager<User> userManager;

        public LoginModel(SignInManager<User> signInManager, UserManager<User> userManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public IList<AuthenticationScheme> ExternalLogins { get; set; }

        public string ReturnUrl { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        public class InputModel
        {
            [Required]
            [StringLength(25, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 3)]
            public string Username { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }

            [Display(Name = "Remember me?")]
            public bool RememberMe { get; set; }
        }

        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid)
            {
				// This doesn't count login failures towards account lockout
				// To enable password failures to trigger account lockout, set lockoutOnFailure: true
				var user = await this.userManager.FindByNameAsync(Input.Username);
				if (user is null)
				{
					TempData["Error-Message"] = "User account does not exist!";
					return RedirectToAction("Index", "Home", new { Area = "" });
				}
				if (user.IsLocked)
				{
					TempData["Error-Message"] = "Your account is locked by an administrator!";
					return RedirectToAction("Index", "Home", new { Area = "" });
				}
				if (user.IsDeleted)
				{
					ModelState.AddModelError(string.Empty, "Invalid login attempt.");
					TempData["Error-Message"] = "User account does not exist!";
					return RedirectToAction("Index", "Home", new { Area = "" });
				}
				var result = await signInManager.PasswordSignInAsync(Input.Username, Input.Password, Input.RememberMe, lockoutOnFailure: true);            
				if (result.Succeeded)
                {
                    var roles = await this.userManager.GetRolesAsync(user);

                    if (roles.Contains("Administrator"))
                    {
                        return RedirectToAction("Index", "Home", new { Area = "Administration" });
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home", new { Area = "" });
                    }
                }
				else
				{
					TempData["Error-Message"] = "Wrong username or password!";
					return RedirectToPage("./Login");
				}
               
            }
            // If we got this far, something failed, redisplay form
            return Page();
        }
    }
}
