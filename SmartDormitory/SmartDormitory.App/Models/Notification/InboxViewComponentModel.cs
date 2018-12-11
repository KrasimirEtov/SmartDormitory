using SmartDormitory.Services.Models.Notifications;
using System.Collections.Generic;

namespace SmartDormitory.App.Models.Notification
{
    public class InboxViewComponentModel
    {
        public int UnseenCount { get; set; } = 0;

        public IEnumerable<InboxServiceModel> Notifications { get; set; }
    }
}
