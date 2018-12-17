using SmartDormitory.Services.Models.Notifications;
using System.Collections.Generic;

namespace SmartDormitory.App.Models.Notification
{
    public class InboxViewModel
    {
        public int TotalPages { get; set; }

        public int CurrentPage { get; set; }

        public int PreviousPage => this.CurrentPage == 1
            ? 1
            : this.CurrentPage - 1;

        public int NextPage => this.CurrentPage == this.TotalPages
            ? this.TotalPages
            : this.CurrentPage + 1;

        public IEnumerable<InboxServiceModel> Notifications { get; set; }
    }
}
