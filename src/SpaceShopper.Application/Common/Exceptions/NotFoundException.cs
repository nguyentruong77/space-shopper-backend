namespace SpaceShopper.Application.Common.Exceptions
{
    public sealed class NotFoundException : BusinessException
    {
        public NotFoundException(string errorCode, string message)
            : base(errorCode, message)
        {
        }
    }
}
