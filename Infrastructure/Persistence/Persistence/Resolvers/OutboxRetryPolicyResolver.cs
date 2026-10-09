namespace Persistence;

using Application;
using Domain;

public class OutboxRetryPolicyResolver : IOutboxRetryPolicyResolver
{
    private readonly IOutboxRetryPolicyRepository _outboxRetryPolicyRepository;

    public OutboxRetryPolicyResolver(IOutboxRetryPolicyRepository outboxRetryPolicyRepository)
    {
        _outboxRetryPolicyRepository = outboxRetryPolicyRepository;
    }

    public async Task<OutboxRetryPolicy> ResolveAsync(string eventType)
    {
        var policyEspecifica = await _outboxRetryPolicyRepository.ObtenerPorEventType(eventType);
        if (policyEspecifica is not null)
            return policyEspecifica;

        return await ResolveDefaultAsync();
    }

    public async Task<OutboxRetryPolicy> ResolveDefaultAsync()
    {
        var policyDefault = await _outboxRetryPolicyRepository.ObtenerDefault();
        return policyDefault ?? OutboxRetryPolicy.CrearDefaultEnMemoria();
    }
}