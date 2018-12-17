using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartDormitory.App.Infrastructure.Extensions;
using SmartDormitory.App.Models.Notification;
using SmartDormitory.Services.Contracts;
using System;
using System.Threading.Tasks;

namespace SmartDormitory.App.Controllers
{
    [Authorize]
    public class NotificationController : Controller
    {
        private const int PageSize = 10;

        private readonly INotificationService notificationService;

        public NotificationController(INotificationService notificationService)
        {
            this.notificationService = notificationService;
        }

        [HttpGet]
        public async Task<IActionResult> Inbox(int seen = 0, int page = 1)
        {
            var userId = this.User.GetId();

            var notifications = await this.notificationService.GetAllByUserId(userId, seen, page, PageSize);
            var totalNotifications = await this.notificationService.TotalCountByCriteria(userId, seen, page, PageSize);

            var model = new InboxViewModel
            {
                CurrentPage = page,
                Notifications = notifications,
                TotalPages = (int)Math.Ceiling(totalNotifications / (double)PageSize),
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string id)
        {
            await this.notificationService.Delete(id);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> ReadAll()
        {
            string userId = this.User.GetId();
            await this.notificationService.ReadAll(userId);

            return RedirectToAction(nameof(Inbox), new { seen = 1 });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAllHistory()
        {
            await this.notificationService.DeleteAllHistory();

            return RedirectToAction(nameof(Inbox), new { seen = 0 });
        }
    }
}
