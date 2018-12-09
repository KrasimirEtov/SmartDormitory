using SmartDormitory.Data.Models.Abstract;

namespace SmartDormitory.Data.Models
{
    public class Notification : BaseEntity
    {
        public string Message { get; set; }

        public User Receiver { get; set; }

        public string ReceiverId { get; set; }

        //add sender too?

        public bool Seen { get; set; } = false;
    }
}
