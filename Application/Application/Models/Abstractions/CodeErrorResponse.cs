namespace Application;

public class CodeErrorResponse
{
    public Guid Id { get; set; }
    public int StatusCode { get; set; }
    public string? Message { get; set; }
    public CodeErrorResponse(int statusCode, string? message = null)
    {
        StatusCode = statusCode;
        Message = message ?? GetDefaultMessageStatusCode(StatusCode);
    }
    private string GetDefaultMessageStatusCode(int statusCode)
    {
        return statusCode switch
        {
            400 => "El Request enviado tiene errores",
            401 => "No tienes authorizacion para este recurso",
            404 => "No se encontró el recurso solicitado",
            500 => "Se produjeron errores en el servidor",
            _ => "Error en la petición"
        };
    }
}
