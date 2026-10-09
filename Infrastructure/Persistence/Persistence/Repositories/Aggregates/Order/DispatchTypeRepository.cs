namespace Persistence;

using Application;
using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

public class DispatchTypeRepository : RepositoryBase<DispatchType>, IDispatchTypeRepository
{
    public DispatchTypeRepository(ApplicationDbContext context) : base(context) { }
    public async Task<IEnumerable<DispatchTypeViewModel>> GetAllDispatchType()
    {
        return await context.DispatchType.AsNoTracking()
                                        .Select(x => new DispatchTypeViewModel
                                        {
                                            Id = x.Id,
                                            Description = x.Description,
                                            IdProvider = x.IdProvider
                                        })
                                        .ToListAsync();
    }
}
