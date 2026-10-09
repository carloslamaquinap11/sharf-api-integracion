using Domain;

namespace Application;

public interface IDispatchTypeRepository : IRepositoryBase<DispatchType>
{
    Task<IEnumerable<DispatchTypeViewModel>> GetAllDispatchType();
}
