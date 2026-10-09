namespace Service;

using System.Security.Claims;
using Application;
using Microsoft.AspNetCore.Http;
public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor httpContextAccessor;
    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var userId = httpContextAccessor.HttpContext?.User.FindFirst("userId")?.Value ?? string.Empty;
            if (string.IsNullOrWhiteSpace(userId) || !Guid.TryParse(userId, out Guid userIdGuid))
            {
                return null;
            }

            return userIdGuid;
        }
    }
    public Guid? AplicacionId
    {
        get
        {
            var aplicacionId = httpContextAccessor.HttpContext?.User.FindFirst("aplicacionId")?.Value ?? string.Empty;
            if (string.IsNullOrWhiteSpace(aplicacionId) || !Guid.TryParse(aplicacionId, out Guid aplicacionIdGuid))
            {
                return null;
            }

            return aplicacionIdGuid;
        }
    }

    public string Email => httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.Email)?.Value ?? string.Empty;

    public string DocumentNumber => httpContextAccessor.HttpContext?.User.FindFirst("documentNumber")?.Value ?? string.Empty;
}