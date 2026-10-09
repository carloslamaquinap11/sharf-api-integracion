namespace Service;

using Application;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

public class MemoryCacheService : IMemoryCacheService
{
    private static readonly int ABSOLUTETIMEMINUTES = 60;
    private readonly IDistributedCache distributedCache;

    public MemoryCacheService(IDistributedCache distributedCache)
    {
        this.distributedCache = distributedCache;
    }
    public async Task Remove(string key)
    {
        await distributedCache.RemoveAsync(key);
    }
    public async Task SetValue<T>(string key, T value)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(ABSOLUTETIMEMINUTES),
        };

        var serializedValue = JsonConvert.SerializeObject(value);

        await distributedCache.SetStringAsync(key, serializedValue, options);
    }
    public async Task SetValue<T>(string key, T value, int minutes)
    {
        var options = new DistributedCacheEntryOptions
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(minutes),
        };

        var serializedValue = JsonConvert.SerializeObject(value);

        await distributedCache.SetStringAsync(key, serializedValue, options);
    }
    public async Task<(bool, T?)> TryGetValue<T>(string key)
    {
        var resultado = await distributedCache.GetStringAsync(key);
        if (!string.IsNullOrEmpty(resultado))
        {
            var value = JsonConvert.DeserializeObject<T>(resultado);
            return (true, value);
        }

        return (false, default);
    }
}