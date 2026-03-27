namespace SpaceShopper.Application.Interfaces.Caching
{
    public interface ICacheService
    {
        Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default);
        Task SetAsync<T>(string key, T value, TimeSpan? AbsoluteExpiration = null, TimeSpan? SlidingExpiration = null, CancellationToken cancellationToken = default);
        Task RemoveAsync(string key, CancellationToken cancellationToken = default);
        Task<bool> ExistsAsync(string key, CancellationToken cancellationToken = default);
        Task<T?> GetAndRemoveAsync<T>(string key, CancellationToken cancellationToken = default);
        Task<(bool Success, T? Value)> TryGetValueAsync<T>(string key, CancellationToken cancellationToken = default);
    }
}
