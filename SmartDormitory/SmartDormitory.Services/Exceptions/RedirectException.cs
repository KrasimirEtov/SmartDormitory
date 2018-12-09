using System;
using System.Collections.Generic;
using System.Text;

namespace SmartDormitory.Services.Exceptions
{
	public class RedirectException : Exception
	{
		public RedirectException(string message) : base(message)
		{

		}
	}
}
