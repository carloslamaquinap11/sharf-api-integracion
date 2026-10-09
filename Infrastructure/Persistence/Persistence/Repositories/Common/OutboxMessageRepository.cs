namespace Persistence;

using Application;
using Domain;
using Microsoft.EntityFrameworkCore;

public class OutboxMessageRepository : RepositoryBase<OutboxMessage>, IOutboxMessageRepository
{
    public OutboxMessageRepository(ApplicationDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<OrderEventViewModel>> GetEventHistoryByValueInPayload(string targetValue)
    {
        return (await context.OutboxMessage
            .AsNoTracking()
            .Where(p => p.IsActive && p.Content.Contains(targetValue, StringComparison.OrdinalIgnoreCase))
            .Select(p => new OrderEventViewModel
            {
                Event = p.Type,
                Payload = p.Content,
                CreatedDate = p.CreatedDate,
                Error = p.Error
            })
            .ToListAsync())
            .OrderByDescending(p => p.CreatedDate);
    }
    public async Task<IEnumerable<OrderEventViewModel>> GetEventHistory()
    {
        return (await context.OutboxMessage
            .AsNoTracking()
            .Where(p => p.IsActive)
            .Select(p => new OrderEventViewModel
            {
                Event = p.Type,
                Payload = p.Content,
                CreatedDate = p.CreatedDate,
                Error = p.Error
            })
            .ToListAsync())
            .OrderByDescending(p => p.CreatedDate);
    }
}