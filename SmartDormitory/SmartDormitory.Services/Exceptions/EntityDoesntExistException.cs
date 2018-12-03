using System;

namespace SmartDormitory.Services.Exceptions
{
    public class EntityDoesntExistException : Exception
    {
        private const string DefaultMessage = "Not existing {0} with id: {1} !";

        public EntityDoesntExistException(string message)
         : base(message)
        {

        }

        public EntityDoesntExistException(string model, string id)
            : base(string.Format(DefaultMessage, model, id))
        {

        }
    }
}
