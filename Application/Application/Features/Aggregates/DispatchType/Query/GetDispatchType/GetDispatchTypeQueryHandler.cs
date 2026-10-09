namespace Application;

using MediatR;

public class GetDispatchTypeQueryHandler(IDispatchTypeRepository dispatchTypeRepository, IMemoryCacheService memoryCacheService) : IRequestHandler<GetDispatchTypeQuery, IEnumerable<DispatchTypeViewModel>>
{
    public async Task<IEnumerable<DispatchTypeViewModel>> Handle(GetDispatchTypeQuery request, CancellationToken cancellationToken)
    {
        var key = "DISPATCHTYPE_VALUES";
        var (exists, list) = await memoryCacheService.TryGetValue<List<DispatchTypeViewModel>>(key);
        if (!exists)
        {
            list = (await dispatchTypeRepository.GetAllDispatchType()).ToList();
            await memoryCacheService.SetValue(key, list);
        }

        return list ?? [];
    }
}