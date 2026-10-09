namespace Application;

using Domain;
public interface IOutboxMessageRepository : IRepositoryBase<OutboxMessage>
{
    Task<IEnumerable<OrderEventViewModel>> GetEventHistoryByValueInPayload(string targetValue);
    Task<IEnumerable<OrderEventViewModel>> GetEventHistory();
}