namespace Persistence;

using Microsoft.Extensions.DependencyInjection;
using System.Collections;
using Application;
using Domain;
using Newtonsoft.Json;
public class UnitOfWork(ApplicationDbContext context, IServiceProvider serviceProvider)
    : IUnitOfWork
{
    private static readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All
    };

    public async Task<int> Complete(bool executeDomainEvent = true)
    {
        try
        {
            var cantRegistrosAfectados = await context.SaveChangesAsync();

            if (!executeDomainEvent) return cantRegistrosAfectados;

            AddDomainEventsAsOutboxMessages();
            await context.SaveChangesAsync();

            return cantRegistrosAfectados;
        }
        catch (Exception e)
        {
            throw new Exception($"Error en Transacción - Message: {e.Message} | InnerException: {e.InnerException} | StackTrace: {e.StackTrace}");
        }
    }

    private void AddDomainEventsAsOutboxMessages()
    {
        var outboxMessages = context.ChangeTracker
            .Entries<BaseDomainModel>()
            .Select(entry => entry.Entity)
            .SelectMany(entity =>
            {
                IReadOnlyList<IDomainEvent> domainEvents = entity.GetDomainEvents();

                entity.ClearDomainEvents();

                return domainEvents;
            })
            .Select(domainEvent => new OutboxMessage(
                Guid.NewGuid(),
                DateTime.UtcNow,
                domainEvent.GetType().Name,
                JsonConvert.SerializeObject(domainEvent, JsonSerializerSettings),
                OutboxMessageStatusEnum.Pending))
            .ToList();

        context.AddRange(outboxMessages);
    }

    public void Dispose()
    {
        context.Dispose();
    }
    public void ClearTrackedChanges()
    {
        context.ChangeTracker.Clear();
    }
    public IRepositoryBase<TEntity> Repository<TEntity>() where TEntity : BaseDomainModel
    {
        return serviceProvider.GetRequiredService<IRepositoryBase<TEntity>>();
    }
}