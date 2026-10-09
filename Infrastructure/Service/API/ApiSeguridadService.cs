namespace Service;

using Application;
using Persistence;
using Domain;

public class ApiSeguridadService(IMemoryCacheService memoryCacheService,
                               IConfigurationRepository configurationRepository,
                               IClientService clientService,
                               IAdminSettings adminSettings,
                               ISettings settings) : ServiceException, IApiSeguridadService
{
    public async Task<string> ObtenerTokenSistema()
    {
        var key = $"ObtenerTokenSistema_ApiIntegracion";
        var (existe, token) = await memoryCacheService.TryGetValue<string>(key);
        if (!existe)
        {
            var admin = await ObtenerLoginAdmin();
            token = admin.AccessToken;
            await memoryCacheService.SetValue(key, token);
        }
        return token!;
    }
    public async Task<LoginViewModel> ObtenerLoginAdmin()
    {
        var path = $"{settings.ApiSeguridad}/api/Auth/Login";
        var adminCredentials = new
        {
            Email = adminSettings.Email,
            Password = adminSettings.Password,
            IdAplicacion = AplicacionEnum.ApiIntegracion.GetId()
        };

        var response = await clientService.PostAsyncJson<LoginViewModel>(path, adminCredentials);

        if (!response.IsSuccess)
            ThrowError(response);

        var login = response.Result
            ?? throw new AuthenticationException(
                response.Message ?? "La respuesta de autenticación no contiene datos.");

        if (!login.IsAuthenticated)
        {
            throw new AuthenticationException(
                response.Message ?? "No se pudo autenticar al usuario.");
        }

        return login;
    }
    private async Task<string> GetValueByKeyFromConfiguration(string key)
    {
        return await configurationRepository.GetValueByKey(key) ?? string.Empty;
    }
    public async Task<string> GetValueByKey(string key, bool almacenarCache = false)
    {
        var (existe, value) = await memoryCacheService.TryGetValue<string>($"Config:{key}");
        if (!existe)
        {
            value = await GetValueByKeyFromConfiguration(key);
            if (almacenarCache && value != string.Empty) await memoryCacheService.SetValue($"Config:{key}", value);
        }
        return value!;
    }
    public async Task<bool> TienePermisoARecurso(IEnumerable<string> roles, string accion, string nombre)
    {
        var path = $"{settings.ApiSeguridad}/api/Auth/TienePermisoARecurso";
        var token = await ObtenerTokenSistema();
        var body = new
        {
            roles = roles,
            accion = accion,
            nombre = nombre
        };
        var response = await clientService.PostAsyncJson<bool>(path, body, "Bearer", token);

        if (!response.IsSuccess)
            ThrowError(response);

        return response.Result;
    }
}