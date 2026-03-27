namespace SpaceShopper.Application.Common.Exceptions
{
    public sealed class DomainException : BusinessException
    {
        public DomainException(string errorCode, string message)
            : base(errorCode, message)
        {
        }
    }
}
