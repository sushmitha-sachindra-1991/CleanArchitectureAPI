using System;

namespace ExtModule.API.Infra.Exceptions
{
    public class CustomException : Exception
    {
        public CustomException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
