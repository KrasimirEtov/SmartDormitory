using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SmartDormitory.App.Models.Notification;
using SmartDormitory.Data.Models;
using SmartDormitory.Services.Contracts;
using System.Threading.Tasks;

namespace SmartDormitory.App.ViewComponents
{
    public class InboxViewComponent : ViewComponent
    {
        private const int LastUnseenCount = 5;

        private readonly INotificationService notificationService;
        private readonly UserManager<User> userManager;

        public InboxViewComponent(INotificationService notificationService, UserManager<User> userManager)
        {
            this.notificationService = notificationService;
            this.userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var userId = this.userManager.GetUserId(Request.HttpContext.User);

            var lastNotifications = await this.notificationService.GetLastUnseenByUserId(userId, LastUnseenCount);
            var notificationsCount = await this.notificationService.GetUnseenCount(userId);

            return View(new InboxViewComponentModel
            {
                UnseenCount = notificationsCount,
                Notifications = lastNotifications
            });
        }
    }
}
