namespace Persistence;

using Application;
using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

public class VehicleRepository : RepositoryBase<Vehicle>, IVehicleRepository
{
    public VehicleRepository(ApplicationDbContext context) : base(context) { }
    public async Task<Vehicle?> GetVehicleByCode(string code, bool asNoTracking = false)
    {
        var query = context.Vehicle.Where(x => x.Code.ToLower() == code.ToLower() && x.IsActive);

        if (asNoTracking)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync();
    }
}
