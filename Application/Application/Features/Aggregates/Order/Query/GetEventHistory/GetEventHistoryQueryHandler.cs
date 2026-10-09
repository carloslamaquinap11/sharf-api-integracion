namespace Application;

using MediatR;
public class GetEventHistoryQueryHandler(IOutboxMessageRepository outboxMessageRepository) : IRequestHandler<GetEventHistoryQuery, IEnumerable<OrderEventViewModel>>
{
    public async Task<IEnumerable<OrderEventViewModel>> Handle(GetEventHistoryQuery request, CancellationToken cancellationToken)
    {
        return await outboxMessageRepository.GetEventHistory();
    }
}