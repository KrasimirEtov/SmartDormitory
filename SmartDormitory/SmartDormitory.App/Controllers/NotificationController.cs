using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartDormitory.App.Infrastructure.Extensions;
using SmartDormitory.Services.Contracts;
using System.Threading.Tasks;

namespace SmartDormitory.App.Controllers
{
    [Authorize]
    public class NotificationController : Controller
    {
        private readonly INotificationService notificationService;

        public NotificationController(INotificationService notificationService)
        {
            this.notificationService = notificationService;
        }

        public async Task<IActionResult> Inbox()
        {
            var userId = this.User.GetId();
            var allNotifications = await this.notificationService.GetAllByUserId(userId);

            return View(allNotifications);
        }


    }
}
