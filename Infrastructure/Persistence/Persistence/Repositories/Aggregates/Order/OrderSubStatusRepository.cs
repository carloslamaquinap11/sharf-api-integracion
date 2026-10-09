namespace Persistence;

using Application;
using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

public class OrderSubStatusRepository : RepositoryBase<OrderSubStatus>, IOrderSubStatusRepository
{
    public OrderSubStatusRepository(ApplicationDbContext context) : base(context) { }
    public async Task<IEnumerable<OrderSubStatusViewModel>> GetAllOrderSubStatus()
    {
        return await context.OrderSubStatus.AsNoTracking()
                                        .Select(x => new OrderSubStatusViewModel
                                        {
                                            Id = x.Id,
                                            Description = x.Description,
                                            IdProvider = x.IdProvider
                                        })
                                        .ToListAsync();
    }
}
