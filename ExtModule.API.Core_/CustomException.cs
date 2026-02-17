namespace ExtModule.API.Core
{
    public class CustomException : Exception
    {

        public CustomException(string message, Exception innerException)
            : base(message, innerException)
        {
        }


    }

}
