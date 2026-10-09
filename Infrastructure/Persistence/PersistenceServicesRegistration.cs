namespace Persistence;

using Application;
using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Microsoft.Extensions.Hosting;

public static class PersistenceServiceRegistration
{
    public static IServiceCollection AddPersistenceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

        services.AddDbContext<ApplicationDbContext>(options => options.UseInMemoryDatabase("IntegracionSharfTest"));

        services.AddHostedService<InMemoryDatabaseSeeder>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped(typeof(IRepositoryBase<>), typeof(RepositoryBase<>));

        services.AddScoped<IClientRepository, ClientRepository>();
        services.AddScoped<ICourierRepository, CourierRepository>();
        services.AddScoped<IDispatchTypeRepository, DispatchTypeRepository>();
        services.AddScoped<IFileRepository, FileRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderStatusRepository, OrderStatusRepository>();
        services.AddScoped<IOrderSubStatusRepository, OrderSubStatusRepository>();
        services.AddScoped<IServiceTypeRepository, ServiceTypeRepository>();
        services.AddScoped<ITrackingRepository, TrackingRepository>();
        services.AddScoped<IVehicleRepository, VehicleRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<INotificationProfileRepository, NotificationProfileRepository>();

        services.AddScoped<IOutboxRetryPolicyResolver, OutboxRetryPolicyResolver>();
        services.AddScoped<IOutboxMessageRepository, OutboxMessageRepository>();
        services.AddScoped<IOutboxRetryPolicyRepository, OutboxRetryPolicyRepository>();
        services.AddScoped<IConfigurationRepository, ConfigurationRepository>();

        AddBackgroundJobs(services, configuration);

        return services;
    }
    private static void AddBackgroundJobs(IServiceCollection services, IConfiguration configuration)
    {
        services.AddQuartz(q =>
        {
            var ProcessOutboxMessagesJobKey = new JobKey(nameof(ProcessOutboxMessagesJob));
            q.AddJob<ProcessOutboxMessagesJob>(opts => opts.WithIdentity(ProcessOutboxMessagesJobKey));


            var intervalInSeconds = int.Parse(configuration["TasksToExecute:IntervalInSeconds"] ?? "10");

            q.AddTrigger(opts => opts
                .ForJob(ProcessOutboxMessagesJobKey)
                .WithIdentity(nameof(ProcessOutboxMessagesJob))
                .WithSimpleSchedule(schedule => schedule
                    .WithInterval(TimeSpan.FromSeconds(intervalInSeconds))
                    .RepeatForever()));

        });

        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);
    }
}

internal sealed class InMemoryDatabaseSeeder(
    IServiceScopeFactory scopeFactory) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        await using var scope = scopeFactory.CreateAsyncScope();

        var db = scope.ServiceProvider
            .GetRequiredService<ApplicationDbContext>();

        await db.Database.EnsureCreatedAsync(cancellationToken);

        if (!await db.OutboxMessage.AnyAsync(cancellationToken))
        {
            db.ServiceType.AddRange(
                ServiceType.Create(Guid.Parse("8255B8EA-CED2-410C-80B1-5F87CF0378D3"), "Recojo del cliente al courier", "PICKUP"),
                ServiceType.Create(Guid.Parse("AA0A828D-B6B0-41A2-B002-298F255FB9C8"), "Entrega al cliente", "LAST_MILE"),
                ServiceType.Create(Guid.Parse("FDAFBE92-98A5-4976-B636-F90078D6459D"), "Devolución al cliente", "RETURN")
            );

            db.DispatchType.AddRange(
                DispatchType.Create(Guid.Parse("9FCAD270-6F37-4227-B34C-136184A0B09B"), "Entrega a domicilio", "HOME_DELIVERY"),
                DispatchType.Create(Guid.Parse("D1BC3D13-7416-4C9B-A699-394ADDE1BB29"), "Recojo en tienda", "STORE_WITHDRAWAL"),
                DispatchType.Create(Guid.Parse("0EA0E0D4-A9DF-4307-95C6-433CDCE5BD88"), "Del consignatario al cliente", "REVERSE")
            );

            db.OrderStatus.AddRange(
                OrderStatus.Create(Guid.Parse("D47453A4-E59F-4B55-8988-3D2A8D1A0B66"), "Pedido asignado a ruta", "PLANNING"),
                OrderStatus.Create(Guid.Parse("BDF723CC-A10B-46AD-B894-404D4CC30429"), "Ruta iniciada por el courier", "STARTED"),
                OrderStatus.Create(Guid.Parse("3F260BD3-1F1A-412E-9C20-35F1C5AD433C"), "Courier llegó al punto de recojo", "AT PICKUP POINT"),
                OrderStatus.Create(Guid.Parse("016FAB8B-93CD-45BC-AE30-1EBF0526CF1C"), "Pedido recogido y validado", "COLLECTED"),
                OrderStatus.Create(Guid.Parse("322D92D2-83C3-4C54-875A-615222E8877E"), "No se pudo recoger el pedido", "NOT COLLECTED"),
                OrderStatus.Create(Guid.Parse("5CCC6562-4727-40FD-AD57-470B07884647"), "Pedido entregado al consignatario", "DELIVERED"),
                OrderStatus.Create(Guid.Parse("EF05602C-BE77-4CF9-A8E5-1ED81A983C61"), "No se pudo completar la entrega", "NOT DELIVERED"),
                OrderStatus.Create(Guid.Parse("6EC8F45B-13BA-4F44-A915-5AB9B96F043C"), "Pedido asignado a devolución", "TO BE RETURN"),
                OrderStatus.Create(Guid.Parse("6CBD507B-7C2E-4D48-AA3C-B4B3F113AC92"), "Pedido devuelto al cliente", "RETURNED"),
                OrderStatus.Create(Guid.Parse("3BF76269-1BC7-44A0-AD09-E5E15CE31B83"), "No se pudo devolver el pedido al cliente", "NOT RETURNED")
            );

            db.OrderSubStatus.AddRange(
                OrderSubStatus.Create(Guid.Parse("A41FA114-E4B0-4732-9D37-505B5F6257C5"), "Cliente recibió su pedido", "CLIENT RECEIVED"),
                OrderSubStatus.Create(Guid.Parse("7B556E63-CB18-41C1-805B-66AA2B56ABA7"), "Dirección no es la que indica el consignatario", "WRONGADDRESS")
            );
            db.Configuration.AddRange(
                Configuration.Create("MAX_VISITS_TO_BE_RETURNED", "3", "Máximo de número de visitas antes de ser asignado para devolución"),
                Configuration.Create("API_INTEGRACION_JOB_OUTBOX_ACTIVADO", "true", "Flag para indicar si el outbox de integración está activado")
            );

            db.NotificationProfile.AddRange(
                NotificationProfile.Create(Guid.Parse("16DE1D41-7396-4EFB-A58B-DB4440B67B2F"),"Default","{\"order\":{{OrderNumber}},\"status\":{{OrderStatus}}}"),
                NotificationProfile.Create(Guid.Parse("C707A3E9-755C-4643-8499-2556FA4B6D01"),"Formato Tiendas Peruanas","{\"orderNumber\":{{OrderNumber}},\"status\":{{OrderStatus}}}"),
                NotificationProfile.Create(Guid.Parse("D111B251-C11E-4227-BAF1-4D2A933D3F99"),"Formato estándar español","{\"pedido\":{{OrderNumber}},\"estado\":{{OrderStatus}}}")
            );

            db.OutboxRetryPolicy.AddRange(
                OutboxRetryPolicy.Create("NotifyChangedOrderStatusDomainEvent",3, 13, BackoffStrategyEnum.Fixed, 5, 1, false)
            );

            await db.SaveChangesAsync(cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) =>
        Task.CompletedTask;
}