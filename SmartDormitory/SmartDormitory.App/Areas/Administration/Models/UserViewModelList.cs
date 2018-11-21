using System.Collections.Generic;

namespace SmartDormitory.App.Areas.Administration.Models
{
	public class UserViewModelList
	{
		public UserViewModelList(IEnumerable<UserViewModel> userViewModels)
		{
			this.UserViewModels = userViewModels;
		}

		public IEnumerable<UserViewModel> UserViewModels { get; set; }
	}
}
