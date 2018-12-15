using Microsoft.AspNetCore.Mvc;

namespace SmartDormitory.App.Controllers
{
    public class ErrorController : Controller
    {

		public IActionResult Index()
		{
			return this.View();
		}

		public IActionResult PageNotFound()
        {
            return this.View();
        }
    }
}
