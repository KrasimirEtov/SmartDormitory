using Microsoft.AspNetCore.Mvc;
using SmartDormitory.App.Models;
using System.Diagnostics;

namespace SmartDormitory.App.Controllers
{
	public class ErrorController : Controller
	{
		public IActionResult PageNotFound()
		{
			return this.View();
		}
	}
}
