using System;

namespace SmartDormitory.Services.Exceptions
{
	public class InvalidClientInputException : Exception
	{
		public InvalidClientInputException(string message) : base(message)
		{

		}
	}
}
