namespace Rest;

using Application;
using BCrypt.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;

public sealed class SecurityAuthorize(IConfiguration configuration) : IAsyncAuthorizationFilter
{
    public Task OnAuthorizationAsync(AuthorizationFilterContext context)
    {
        var clientId = context.HttpContext.Request
            .Headers["x-client-id"].ToString();

        var clientSecret = context.HttpContext.Request
            .Headers["x-client-secret"].ToString();

        var secretHash = configuration[$"TestClients:{clientId}:SecretHash"];

        var valid = !string.IsNullOrWhiteSpace(clientId)
            && !string.IsNullOrWhiteSpace(clientSecret)
            && !string.IsNullOrWhiteSpace(secretHash)
            && BCrypt.Verify(clientSecret, secretHash);

        if (!valid)
        {
            context.Result = new UnauthorizedObjectResult(
                new { message = "Credenciales inválidas." });
        }

        return Task.CompletedTask;
    }
}