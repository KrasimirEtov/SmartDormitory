using SmartDormitory.Data.Models;
using SmartDormitory.Services.Models.Users;

namespace SmartDormitory.App.Areas.Administration.Models
{
	public class UserViewModel
	{
		public UserViewModel()
		{

		}

		public UserViewModel(UserListServiceModel user)
		{
			Id = user.Id;
			FirstName = user.FirstName;
			LastName = user.LastName;
			UserName = user.UserName;
			SensorsCount = user.SensorsCount;
			IsDeleted = user.IsDeleted;
		}

		public string Id { get; set; }
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string UserName { get; set; }
		public int SensorsCount { get; set; }
		public bool IsAdmin { get; set; }
		public bool IsDeleted { get; set; }

	}
}
