namespace Service;

using Application;
using Domain;
using Newtonsoft.Json;
public class EmailService(
    IApiSeguridadService apiSeguridadService,
    IClientService clientService
    ) : ServiceException, IEmailService
{
    public async Task<string> GetUrlBase()
    {
        var url = await apiSeguridadService.GetValueByKey("URL_API_EMAIL");
        return url;
    }
    public async Task<bool> NotificacionErrorOutboxMessage(Guid id, string type, string mensajeError)
    {
        var url = await GetUrlBase();
        var path = $"{url}/api/Email/OutboxMessage/Error";
        var token = await apiSeguridadService.ObtenerTokenSistema();

        var subject = $"Error en OutboxMessage: {type} (Id: {id})";
        var body = new
        {
            Subject = subject,
            Detail = mensajeError,
        };

        var response = await clientService.PostAsyncJson<bool>(path, body, "Bearer", token);

        if (!response.IsSuccess)
            ThrowError(response);

        return response.Result;
    }
}