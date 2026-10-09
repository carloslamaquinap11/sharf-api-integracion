namespace Application;

using Domain;
public interface IOutboxRetryPolicyResolver
{
    Task<OutboxRetryPolicy> ResolveAsync(string eventType);
    Task<OutboxRetryPolicy> ResolveDefaultAsync();
}