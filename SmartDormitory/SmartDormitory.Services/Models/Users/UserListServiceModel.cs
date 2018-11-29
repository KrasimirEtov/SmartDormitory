
namespace SmartDormitory.Services.Models.Users
{
	public class UserListServiceModel
	{
		public string Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string UserName { get; set; }
		public int SensorsCount { get; set; }
		public bool IsDeleted { get; set; }
	}
}
