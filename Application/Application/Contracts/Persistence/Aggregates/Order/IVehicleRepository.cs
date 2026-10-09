using Domain;

namespace Application;

public interface IVehicleRepository : IRepositoryBase<Vehicle>
{
    Task<Vehicle?> GetVehicleByCode(string code, bool asNoTracking = false);
}
