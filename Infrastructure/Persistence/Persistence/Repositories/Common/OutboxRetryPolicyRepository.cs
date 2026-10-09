namespace Persistence;

using Application;
using Domain;
using Microsoft.EntityFrameworkCore;

public class OutboxRetryPolicyRepository : RepositoryBase<OutboxRetryPolicy>, IOutboxRetryPolicyRepository
{
    public OutboxRetryPolicyRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<OutboxRetryPolicy?> ObtenerPorEventType(string eventType)
    {
        return await context.OutboxRetryPolicy
            .AsNoTracking()
            .Where(p => p.IsActive && p.EventType == eventType)
            .FirstOrDefaultAsync();
    }

    public async Task<OutboxRetryPolicy?> ObtenerDefault()
    {
        return await ObtenerPorEventType(OutboxRetryPolicy.DefaultEventType);
    }
}