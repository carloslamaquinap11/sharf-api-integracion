namespace Application;

using MediatR;
public class GetEventHistoryByOrderNumberQueryHandler(IOutboxMessageRepository outboxMessageRepository, IOrderRepository orderRepository) : IRequestHandler<GetEventHistoryByOrderNumberQuery, IEnumerable<OrderEventViewModel>>
{
    public async Task<IEnumerable<OrderEventViewModel>> Handle(GetEventHistoryByOrderNumberQuery request, CancellationToken cancellationToken)
    {
        var existsOrder = await orderRepository.ExistsOrderByNumber(request.OrderNumber);
        if (!existsOrder)
            return [];

        return await outboxMessageRepository.GetEventHistoryByValueInPayload(request.OrderNumber);
    }
}