namespace Persistence;

using Application;
using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

public class ServiceTypeRepository : RepositoryBase<ServiceType>, IServiceTypeRepository
{
    public ServiceTypeRepository(ApplicationDbContext context) : base(context) { }
    public async Task<IEnumerable<ServiceTypeViewModel>> GetAllServiceType()
    {
        return await context.ServiceType.AsNoTracking()
                                        .Select(x => new ServiceTypeViewModel
                                        {
                                            Id = x.Id,
                                            Description = x.Description,
                                            IdProvider = x.IdProvider
                                        })
                                        .ToListAsync();
    }
}
