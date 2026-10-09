using Domain;

namespace Application;

public interface IClientRepository: IRepositoryBase<Client>
{
    Task<Client?> GetClientByCode(string code, bool asNoTracking = false);
}
