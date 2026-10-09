using Domain;

namespace Application;

public interface IOrderSubStatusRepository : IRepositoryBase<OrderSubStatus>
{
    Task<IEnumerable<OrderSubStatusViewModel>> GetAllOrderSubStatus();
}
