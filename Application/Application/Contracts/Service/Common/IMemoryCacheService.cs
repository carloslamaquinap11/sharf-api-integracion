namespace Application
{
    public interface IMemoryCacheService
    {
        Task Remove(string key);
        Task SetValue<T>(string key, T value);
        Task SetValue<T>(string key, T value, int minutes);
        Task<(bool, T?)> TryGetValue<T>(string key);
    }
}
