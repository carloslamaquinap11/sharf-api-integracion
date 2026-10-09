using Domain;

namespace Application;

public interface IOrderRepository : IRepositoryBase<Order>
{
    Task<bool> ExistsOrderByNumber(string orderNumber);
    Task<Order?> GetOrderByNumber(string orderNumber);
}
