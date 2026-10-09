namespace Service;

using Application;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public static class ServicesServiceRegistration
{
    public static IServiceCollection AddServiceServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddDistributedMemoryCache();
        services.AddLogging(logging =>
        {
            logging.ClearProviders();
        });

        services.AddSingleton<ILoggerService, LoggerService>();

        services.AddHttpClient();

        var adminSettings = new AdminSettings();
        configuration.GetSection(nameof(AdminSettings)).Bind(adminSettings);
        services.AddSingleton<IAdminSettings>(adminSettings);
        var settings = new Settings();
        configuration.GetSection("Settings").Bind(settings);
        services.AddSingleton<ISettings>(settings);

        services.AddScoped<IApiSeguridadService, ApiSeguridadService>();
        services.AddScoped<IClientService, ClientService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IDateTimeService, DateTimeService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IMemoryCacheService, MemoryCacheService>();
        services.AddScoped<IEmailService, EmailService>();

        return services;
    }
}