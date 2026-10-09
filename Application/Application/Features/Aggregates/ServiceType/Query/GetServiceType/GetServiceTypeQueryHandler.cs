namespace Application;

using MediatR;

public class GetServiceTypeQueryHandler(IServiceTypeRepository serviceTypeRepository, IMemoryCacheService memoryCacheService) : IRequestHandler<GetServiceTypeQuery, IEnumerable<ServiceTypeViewModel>>
{
    public async Task<IEnumerable<ServiceTypeViewModel>> Handle(GetServiceTypeQuery request, CancellationToken cancellationToken)
    {
        var key = "SERVICETYPE_VALUES";
        var (exists, list) = await memoryCacheService.TryGetValue<List<ServiceTypeViewModel>>(key);
        if (!exists)
        {
            list = (await serviceTypeRepository.GetAllServiceType()).ToList();
            await memoryCacheService.SetValue(key, list);
        }

        return list ?? [];
    }
}