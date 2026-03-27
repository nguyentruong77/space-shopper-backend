namespace SpaceShopper.Application.Common.Exceptions
{
    public abstract class BusinessException : Exception
    {
        public string ErrorCode { get; }
        public object? Details { get; }

        protected BusinessException(string errorCode, string message, object? details = null)
            : base(message)
        {
            ErrorCode = errorCode;
            Details = details;
        }
    }
}
