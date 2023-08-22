namespace Payment_Gateway.Models.Exceptions
{
    public abstract class BadRequestException : Exception
    {
        protected BadRequestException(string message)
                : base(message)
        {

        }
    }

    public sealed class IdParametersBadRequestException : BadRequestException
    {
        public IdParametersBadRequestException()
            : base("Parameter ids is null")
        {
        }
    }
}
