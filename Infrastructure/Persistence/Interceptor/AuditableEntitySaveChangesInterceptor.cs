namespace Persistence;


using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Threading;
using Domain;
using Application;

public class AuditableEntitySaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly ICurrentUserService currentUserService;
    private readonly IDateTimeService dateTimeService;

    public AuditableEntitySaveChangesInterceptor(ICurrentUserService currentUserService, IDateTimeService dateTimeService)
    {
        this.currentUserService = currentUserService;
        this.dateTimeService = dateTimeService;
    }
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);
        base.SavingChangesAsync(eventData, result);
        OnAfterSaveChanges(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);
        await base.SavingChangesAsync(eventData, result, cancellationToken);
        OnAfterSaveChanges(eventData.Context);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public void UpdateEntities(DbContext? context)
    {
        if (context == null) return;

        foreach (var entry in context.ChangeTracker.Entries<BaseDomainModel>())
        {
            var userConnected = currentUserService.UserId is null ? "system" : currentUserService.UserId.Value.ToString();
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.SetIsActive(true);
                    entry.Entity.SetCreatedAuditory(userConnected, dateTimeService.AmericaLimaTimeZone);
                    break;
                case EntityState.Modified:
                    entry.Entity.SetUpdatedAuditory(userConnected, dateTimeService.AmericaLimaTimeZone);
                    break;

                case EntityState.Detached:
                    break;
                case EntityState.Unchanged:
                    break;
                case EntityState.Deleted:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public void OnAfterSaveChanges(DbContext? context)
    {
        if (context == null) return;
        var entries = context.ChangeTracker.Entries().ToArray();
        var userConnected = currentUserService.UserId is null ? "system" : currentUserService.UserId.Value.ToString();
        foreach (var entry in entries)
        {
            if (entry.Entity is not Auditory && entry.Entity is BaseDomainModel entity)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        var auditoryCreate = Auditory.Create(
                            entry.Metadata.GetTableName() ?? "",
                            entity.Id,
                            dateTimeService.AmericaLimaTimeZone,
                            OperationTypeEnum.Added,
                            null,
                            JsonSerializer.Serialize(entry.CurrentValues.ToObject()),
                            userConnected
                        );
                        auditoryCreate.SetCreatedAuditory(userConnected, dateTimeService.AmericaLimaTimeZone);

                        context.Set<Auditory>().Add(auditoryCreate);
                        break;
                    case EntityState.Modified:
                        var auditoryModified = Auditory.Create(
                            entry.Metadata.GetTableName() ?? "",
                            entity.Id,
                            dateTimeService.AmericaLimaTimeZone,
                            OperationTypeEnum.Modified,
                            JsonSerializer.Serialize(entry.OriginalValues.ToObject()),
                            JsonSerializer.Serialize(entry.CurrentValues.ToObject()),
                            userConnected
                        );
                        auditoryModified.SetCreatedAuditory(userConnected, dateTimeService.AmericaLimaTimeZone);

                        context.Set<Auditory>().Add(auditoryModified);
                        break;

                    case EntityState.Detached:
                        break;
                    case EntityState.Unchanged:
                        break;
                    case EntityState.Deleted:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }


}
