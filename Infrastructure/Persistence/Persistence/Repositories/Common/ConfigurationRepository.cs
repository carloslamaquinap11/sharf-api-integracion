namespace Persistence;

using Application;
using Domain;
using Microsoft.EntityFrameworkCore;

public class ConfigurationRepository : RepositoryBase<Configuration>, IConfigurationRepository
{
    public ConfigurationRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<string?> GetValueByKey(string key)
    {
        return await context.Configuration.AsNoTracking().Where(x => x.Key == key && x.IsActive).Select(x => x.Value).FirstOrDefaultAsync();
    }
}