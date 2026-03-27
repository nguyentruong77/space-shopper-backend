namespace SpaceShopper.Application.Common.Exceptions
{
    public sealed class UnauthorizedException : BusinessException
    {
        public UnauthorizedException(string errorCode, string message)
            : base(errorCode, message)
        {
        }
    }
}
