namespace SpaceShopper.Application.Common.Exceptions
{
    public sealed class ValidationException : BusinessException
    {
        public ValidationException(string errorCode, string message, object? details = null)
            : base(errorCode, message, details)
        {
        }
    }
}
