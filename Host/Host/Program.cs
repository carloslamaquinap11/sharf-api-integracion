using Persistence;
using Service;
using Rest;
using System.Reflection;
using Microsoft.OpenApi;

static string GetBuildVersion()
{
    var asm = Assembly.GetExecutingAssembly();

    var info = asm.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
    if (!string.IsNullOrWhiteSpace(info))
        return info;

    return asm.GetName().Version?.ToString() ?? "0.0.0-dev";
}

var builder = WebApplication.CreateBuilder(args);

var buildVersion = GetBuildVersion();

builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "API Integración",
        Version = buildVersion
    });

    c.AddSecurityDefinition("ClientId", new OpenApiSecurityScheme
    {
        Description = "Client ID del proveedor.",
        Name = "x-client-id",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });

    c.AddSecurityDefinition("ClientSecret", new OpenApiSecurityScheme
    {
        Description = "Client secret del proveedor.",
        Name = "x-client-secret",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey
    });

    c.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("ClientId", document)] = [],
            [new OpenApiSecuritySchemeReference("ClientSecret", document)] = []
        });
});

#region Agregar los servicios 

builder.Services.AddHealthChecks();
builder.Services.AddHttpContextAccessor();
builder.Services.AddPersistenceServices(builder.Configuration);
builder.Services.AddServiceServices(builder.Configuration);
builder.Services.AddRestServices();

var presentationAssembly = typeof(RestServiceRegistration).Assembly;
builder.Services.AddControllers().AddApplicationPart(presentationAssembly);

#endregion

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        policyBuilder =>
        {
            policyBuilder.AllowAnyOrigin()
                   .AllowAnyHeader()
                   .AllowAnyMethod();
        });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", $"API NSV"));
}

app.UseCors("AllowSpecificOrigins");

app.UseHsts();

app.UseRouting();

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UsePresentationMiddleware();

app.MapGet("/", () => Results.Ok()).AllowAnonymous();

app.MapHealthChecks("/health").AllowAnonymous();

app.MapGet("/version", () => Results.Ok(new { version = buildVersion }))
    .AllowAnonymous();

app.MapControllers();

await app.RunAsync();