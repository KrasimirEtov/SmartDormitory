using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartDormitory.App.Areas.Administration.Models
{
	public class UsersPagingViewModel
	{
		public IEnumerable<UserViewModel> Users { get; set; }
		public int CurrentPage { get; set; }
		public int TotalPages { get; set; }
		public int PreviousPage => this.CurrentPage == 1 ? 1 : this.CurrentPage - 1;
		public int NextPage => this.CurrentPage == TotalPages ? TotalPages : this.CurrentPage + 1;
	}
}
