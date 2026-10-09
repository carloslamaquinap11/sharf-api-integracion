namespace Application;

using Domain;
public interface IOutboxRetryPolicyRepository : IRepositoryBase<OutboxRetryPolicy>
{
    Task<OutboxRetryPolicy?> ObtenerPorEventType(string eventType);
    Task<OutboxRetryPolicy?> ObtenerDefault();
}