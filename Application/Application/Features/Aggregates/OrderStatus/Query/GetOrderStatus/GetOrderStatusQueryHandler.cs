namespace Application;

using MediatR;

public class GetOrderStatusQueryHandler(IOrderStatusRepository orderStatusRepository, IMemoryCacheService memoryCacheService) : IRequestHandler<GetOrderStatusQuery, IEnumerable<OrderStatusViewModel>>
{
    public async Task<IEnumerable<OrderStatusViewModel>> Handle(GetOrderStatusQuery request, CancellationToken cancellationToken)
    {
        var key = "ORDERSTATUS_VALUES";
        var (exists, list) = await memoryCacheService.TryGetValue<List<OrderStatusViewModel>>(key);
        if (!exists)
        {
            list = (await orderStatusRepository.GetAllOrderStatus()).ToList();
            await memoryCacheService.SetValue(key, list);
        }

        return list ?? [];
    }
}