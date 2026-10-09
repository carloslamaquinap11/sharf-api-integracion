namespace Application;

using Domain;
public interface IConfigurationRepository : IRepositoryBase<Configuration>
{
    Task<string?> GetValueByKey(string key);
}