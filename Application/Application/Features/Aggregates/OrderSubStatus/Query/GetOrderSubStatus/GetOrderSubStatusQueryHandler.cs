namespace Application;

using MediatR;

public class GetOrderSubStatusQueryHandler(IOrderSubStatusRepository orderSubStatusRepository, IMemoryCacheService memoryCacheService) : IRequestHandler<GetOrderSubStatusQuery, IEnumerable<OrderSubStatusViewModel>>
{
    public async Task<IEnumerable<OrderSubStatusViewModel>> Handle(GetOrderSubStatusQuery request, CancellationToken cancellationToken)
    {
        var key = "ORDERSUBSTATUS_VALUES";
        var (exists, list) = await memoryCacheService.TryGetValue<List<OrderSubStatusViewModel>>(key);
        if (!exists)
        {
            list = (await orderSubStatusRepository.GetAllOrderSubStatus()).ToList();
            await memoryCacheService.SetValue(key, list);
        }

        return list ?? [];
    }
}