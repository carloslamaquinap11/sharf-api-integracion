namespace Rest;

using System.Linq;
using Application;
using Domain;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
public static class RestServiceRegistration
{
    public static IServiceCollection AddRestServices(this IServiceCollection services)
    {
        services.AddApplicationServices();
        services.AddScoped<SecurityAuthorize>();
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errores = context.ModelState.Where(e => e.Value!.Errors.Count > 0);
                var errorMessage = string.Join(' ', errores.Select(x => string.Join(' ', x.Value!.Errors.Select(e => e.ErrorMessage))));

                throw new InvalidElementException(errorMessage);
            };
        });

        return services;
    }
    public static IApplicationBuilder UsePresentationMiddleware(this IApplicationBuilder app)
    {
        app.UseMiddleware<ExceptionMiddleware>();
        return app;
    }
}
