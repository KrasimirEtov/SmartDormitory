using System;
using System.Collections.Generic;
using System.Text;

namespace SmartDormitory.Services.Exceptions
{
	public class EntityAlreadyExistsException : Exception
	{
		public EntityAlreadyExistsException(string message) : base(message)
		{

		}
	}
}
