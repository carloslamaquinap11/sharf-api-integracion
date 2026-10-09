namespace Service;

using Application;
using Newtonsoft.Json;
using System.Net;
public class ServiceException
{
    public ServiceException() { }
    public void ThrowError<T>(ResponseModel<T> response)
    {
        var responseMessage = response.Message ?? "Sin mensaje capturado";

        ErrorClientProviderDetails? error;

        try
        {
            if (response.StatusCode == (int)HttpStatusCode.NotFound)
                throw new Exception();

            error = JsonConvert.DeserializeObject<ErrorClientProviderDetails>(
                responseMessage);
        }
        catch (Exception)
        {
            throw new ClientException(
                responseMessage
                    .Replace("\\\\", "\\")
                    .Replace("\\\"", "\""));
        }

        throw new IntegrationException(
            response.StatusCode,
            error?.Response?.Details?.ToString() ?? responseMessage);
    }
}