using Microsoft.AspNetCore.Mvc;

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
