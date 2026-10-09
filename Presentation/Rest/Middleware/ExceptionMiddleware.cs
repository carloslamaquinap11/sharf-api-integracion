namespace Rest;

using Domain;
using Application;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net;
using System.Diagnostics;
using System.Text;
public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILoggerService _logger;
    public ExceptionMiddleware(RequestDelegate next, ILoggerService logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, IServiceProvider serviceProvider)
    {
        var idRequest = Guid.NewGuid();
        var body = "";

        var requestBodyStream = new MemoryStream();
        await context.Request.Body.CopyToAsync(requestBodyStream);
        requestBodyStream.Seek(0, SeekOrigin.Begin);
        var requestBodyText = await new StreamReader(requestBodyStream).ReadToEndAsync();
        requestBodyStream.Seek(0, SeekOrigin.Begin);

        body = requestBodyText.Replace("\\\"", "\"");
        context.Request.Body = requestBodyStream;

        var relojRequest = new Stopwatch();
        relojRequest.Start();

        try
        {
            await _logger.LogInfo($"idRequest: {idRequest}", $"{context.Request.Scheme}://{context.Request.Host + context.Request.Path}", context.Request.Method, body);
            await _next(context);
        }
        catch (Exception ex)
        {
            await _logger.LogError($" || idRequest: {idRequest} || statusCode: - || exception ({ex.GetType().Name}): {ex.Message} || {ex.InnerException}");
            var newResponseBody = new MemoryStream();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            var originalResponseBody = context.Response.Body;

            context.Response.Body = newResponseBody;

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            var statusCode = (int)HttpStatusCode.InternalServerError;
            CodeErrorException result;

            switch (ex)
            {
                case IntegrationException integrationException:
                    statusCode = integrationException.StatusCode;
                    result = new CodeErrorException(integrationException.StatusCode, nameof(IntegrationException), integrationException.Message.ToString());
                    break;
                case UnauthorizedAccessException unauthorizedAccessException:
                    statusCode = (int)HttpStatusCode.Unauthorized;
                    result = new CodeErrorException(statusCode, nameof(UnauthorizedAccessException), unauthorizedAccessException.Message.ToString());
                    break;
                case NotFoundException notFoundException:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    result = new CodeErrorException(statusCode, nameof(NotFoundException), notFoundException.Message.ToString());
                    break;
                case InvalidElementException invalidElementException:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    result = new CodeErrorException(statusCode, nameof(InvalidElementException), ex.Message.ToString());
                    break;
                case FormatException formatException:
                    statusCode = (int)HttpStatusCode.BadRequest;
                    result = new CodeErrorException(statusCode, nameof(FormatException), ex.Message.ToString() + $" Value should be '{formatException.TargetSite?.DeclaringType?.FullName}' type.");
                    break;
                // case AuthenticationException authenticationException:
                //     statusCode = (int)HttpStatusCode.InternalServerError;
                //     result = new CodeErrorException(statusCode, nameof(AuthenticationException), ex.Message);
                //     break;
                case ClientException clientException:
                    statusCode = (int)HttpStatusCode.InternalServerError;
                    result = new CodeErrorException(statusCode, nameof(ClientException), clientException.Message);
                    break;
                case ApplicationException applicationException:
                    statusCode = (int)HttpStatusCode.InternalServerError;
                    result = new CodeErrorException(statusCode, nameof(ApplicationException), ex.Message);
                    break;
                default:
                    statusCode = (int)HttpStatusCode.InternalServerError;
                    result = new CodeErrorException(statusCode, nameof(Exception), ex.Message + ex.StackTrace);
                    break;
            }

            var resultadoString = JsonConvert.SerializeObject(result);

            context.Response.Body = originalResponseBody;
            context.Response.StatusCode = statusCode;
            await context.Response.WriteAsync(resultadoString);
            stopwatch.Stop();

            if (newResponseBody != null)
            {
                newResponseBody.Close();
                newResponseBody.Dispose();
            }

            await _logger.LogError($" || idRequest: {idRequest} || statusCode: {statusCode} || exception ({ex.GetType().Name}): {ex.Message} || {ex.InnerException}");
        }
        finally
        {
            relojRequest.Stop();
            await _logger.LogInfo($" || idRequest: {idRequest} || duración: {relojRequest.ElapsedMilliseconds}ms");
        }
    }
}