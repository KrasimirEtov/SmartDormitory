using System;

namespace SmartDormitory.Services.Models.Notifications
{
    public class InboxServiceModel
    {
        public string Title { get; set; }

        public float AlarmValue { get; set; }

        public DateTime CreatedOn { get; set; }
    }
}
