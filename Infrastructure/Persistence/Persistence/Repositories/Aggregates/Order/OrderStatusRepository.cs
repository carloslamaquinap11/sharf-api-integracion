namespace Persistence;

using Application;
using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

public class OrderStatusRepository : RepositoryBase<OrderStatus>, IOrderStatusRepository
{
    public OrderStatusRepository(ApplicationDbContext context) : base(context) { }
    public async Task<IEnumerable<OrderStatusViewModel>> GetAllOrderStatus()
    {
        return await context.OrderStatus.AsNoTracking()
                                        .Select(x => new OrderStatusViewModel
                                        {
                                            Id = x.Id,
                                            Description = x.Description,
                                            IdProvider = x.IdProvider
                                        })
                                        .ToListAsync();
    }
}
