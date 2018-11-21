using System;
using System.Collections.Generic;
using System.Text;

namespace SmartDormitory.Services.Exceptions
{
	public class EntityDoesntExistException : Exception
	{
		public EntityDoesntExistException(string message) : base(message)
		{

		}
	}
}
