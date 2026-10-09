namespace Persistence;

using Application;
using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

public class TrackingRepository : RepositoryBase<Tracking>, ITrackingRepository
{
    public TrackingRepository(ApplicationDbContext context) : base(context) { }
    public async Task<Tracking?> GetTrackingByNumber(string trackingNumber)
    {
        return await context.Tracking.FirstOrDefaultAsync(p => p.Number == trackingNumber && p.IsActive);
    }
}
