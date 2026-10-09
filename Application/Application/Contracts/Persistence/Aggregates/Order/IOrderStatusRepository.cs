using Domain;

namespace Application;

public interface IOrderStatusRepository : IRepositoryBase<OrderStatus>
{
    Task<IEnumerable<OrderStatusViewModel>> GetAllOrderStatus();
}
