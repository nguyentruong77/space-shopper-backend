namespace SpaceShopper.Application.Interfaces.Security
{
    public interface ICacheKeyHashService
    {
        string Hash(string input);
    }
}
