namespace Persistence;

using Application;
using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

public class OrderRepository : RepositoryBase<Order>, IOrderRepository
{
    public OrderRepository(ApplicationDbContext context) : base(context) { }

    public async Task<Order?> GetOrderByIdAndStatus(Guid id, Guid idOrderStatus, bool asNoTracking = false)
    {
        var query = context.Order.Where(x => x.Id == id && x.IdOrderStatus == idOrderStatus && x.IsActive);

        if (asNoTracking) query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync();
    }
    public async Task<bool> ExistsOrderByNumber(string orderNumber)
    {
        return await context.Order.AnyAsync(p => p.Number == orderNumber && p.IsActive);
    }
    public async Task<Order?> GetOrderByNumber(string orderNumber)
    {
        return await context.Order.FirstOrDefaultAsync(p => p.Number == orderNumber && p.IsActive);
    }
}
