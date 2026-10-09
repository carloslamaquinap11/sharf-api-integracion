namespace Persistence;

using Application;
using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

public class CourierRepository : RepositoryBase<Courier>, ICourierRepository
{
    public CourierRepository(ApplicationDbContext context) : base(context) { }
    public async Task<Courier?> GetCourierByName(string name, bool asNoTracking = false)
    {
        var query = context.Courier.Where(x => x.Name.ToLower() == name.ToLower() && x.IsActive);

        if (asNoTracking)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync();
    }
}
