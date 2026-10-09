using Domain;

namespace Application;

public interface ITrackingRepository : IRepositoryBase<Tracking>
{
    Task<Tracking?> GetTrackingByNumber(string trackingNumber);
}
