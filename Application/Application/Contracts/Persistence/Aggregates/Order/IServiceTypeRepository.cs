using Domain;

namespace Application;

public interface IServiceTypeRepository : IRepositoryBase<ServiceType>
{
    Task<IEnumerable<ServiceTypeViewModel>> GetAllServiceType();
}
