namespace Persistence;

using Domain;
using Microsoft.EntityFrameworkCore;
using System.Reflection;
public class ApplicationDbContext : DbContext
{
    public readonly AuditableEntitySaveChangesInterceptor AuditableEntitySaveChangesInterceptor;
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options,
            AuditableEntitySaveChangesInterceptor auditableEntitySaveChangesInterceptor
        ) : base(options)
    {
        AuditableEntitySaveChangesInterceptor = auditableEntitySaveChangesInterceptor;
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        base.OnModelCreating(modelBuilder);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddInterceptors(AuditableEntitySaveChangesInterceptor);
        optionsBuilder.EnableSensitiveDataLogging();
    }
    public required DbSet<Client> Client { get; set; }
    public required DbSet<Courier> Courier { get; set; }
    public required DbSet<DispatchType> DispatchType { get; set; }
    public required DbSet<File> File { get; set; }
    public required DbSet<Order> Order { get; set; }
    public required DbSet<OrderStatus> OrderStatus { get; set; }
    public required DbSet<OrderSubStatus> OrderSubStatus { get; set; }
    public required DbSet<ServiceType> ServiceType { get; set; }
    public required DbSet<Tracking> Tracking { get; set; }
    public required DbSet<Vehicle> Vehicle { get; set; }
    public required DbSet<Notification> Notification { get; set; }
    public required DbSet<NotificationProfile> NotificationProfile { get; set; }

    public required DbSet<Auditory> Auditory { get; set; }
    public required DbSet<Configuration> Configuration { get; set; }
    public required DbSet<OutboxMessage> OutboxMessage { get; set; }
    public required DbSet<OutboxRetryPolicy> OutboxRetryPolicy { get; set; }
}