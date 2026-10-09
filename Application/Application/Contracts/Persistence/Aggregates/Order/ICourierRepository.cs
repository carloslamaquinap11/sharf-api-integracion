using Domain;

namespace Application;

public interface ICourierRepository: IRepositoryBase<Courier>
{
    Task<Courier?> GetCourierByName(string name, bool asNoTracking = false);
}
