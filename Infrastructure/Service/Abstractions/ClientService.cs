namespace Service;

using Application;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Net;
using System.Text;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Runtime;

public class ClientService : IClientService
{
    private readonly ISettings _settings;
    public ClientService(ISettings settings)
    {
        _settings = settings;
    }

    public async Task<ResponseModel<T>> ExecuteApiCall<T>(Func<Task<ResponseModel<T>>> apiOperation)
    {
        try
        {
            return await apiOperation();
        }
        catch (TaskCanceledException taskCanceledException)
        {
            return GenerateExceptionResponse<T>($"TaskCanceledException: {taskCanceledException.Message}");
        }
        catch (JsonReaderException jsonReaderException)
        {
            return GenerateExceptionResponse<T>($"JsonReaderException: {jsonReaderException.Message}");
        }
        catch (ArgumentNullException argumentNullException)
        {
            return GenerateExceptionResponse<T>($"ArgumentNullException: {argumentNullException.Message}");
        }
        catch (ArgumentException argumentException)
        {
            return GenerateExceptionResponse<T>($"ArgumentException: {argumentException.Message}");
        }
        catch (WebException webException)
        {
            return GenerateExceptionResponse<T>($"WebException: {webException.Message}");
        }
        catch (InvalidOperationException invalidOperationException)
        {
            return GenerateExceptionResponse<T>($"InvalidOperationException: {invalidOperationException.Message}");
        }
        catch (TimeoutException timeoutException)
        {
            return GenerateExceptionResponse<T>($"TimeoutException: {timeoutException.Message}");
        }
        catch (IOException ioException)
        {
            return GenerateExceptionResponse<T>($"IOException: {ioException.Message}");
        }
        catch (UnauthorizedAccessException unauthorizedAccessException)
        {
            return GenerateExceptionResponse<T>($"UnauthorizedAccessException: {unauthorizedAccessException.Message}");
        }
        catch (HttpRequestException httpRequestException)
        {

            string detailedMessage = $"HttpRequestException: {httpRequestException.Message}";

            if (httpRequestException.InnerException != null)
            {
                detailedMessage += $" <---> Inner Exception: {httpRequestException.InnerException.Message}";
            }

            return GenerateExceptionResponse<T>(detailedMessage);
        }
        catch (Exception ex)
        {
            return GenerateExceptionResponse<T>($"Unhandled Exception: {ex.Message}");
        }
    }

    public Task<ResponseModel<T>> GetAsync<T>(string path,
        string? tokenType = null, string? accessToken = null)
    {
        return ExecuteApiCall(async () =>
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(60);

            if (!string.IsNullOrEmpty(tokenType) && !string.IsNullOrEmpty(accessToken))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, accessToken);
            }

            var url = $"{path}";

            var ResponseModel = await ExecuteClient(client, url, HttpMethod.Get);

            if (typeof(T) == typeof(Stream))
            {
                Stream stream = await ResponseModel.Content.ReadAsStreamAsync();

                return GenerateSuccessResponse<T>((T)(object)stream);
            }
            else
            {
                var answer = await ResponseModel.Content.ReadAsStringAsync();
                return !ResponseModel.IsSuccessStatusCode
                ? GenerateErrorResponse<T>(answer, ResponseModel)
                    : GenerateSuccessResponse<T>(answer);
            }
        });
    }

    public Task<ResponseModel<T>> GetAsyncWithQueryParams<T>(string path, Dictionary<string, string> queryParams,
        string? tokenType = null, string? accessToken = null)
    {
        return ExecuteApiCall(async () =>
        {
            string queryParamsCollection = string.Join("&", queryParams.Select(x => $"{x.Key}={x.Value}"));
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(60);
            if (!string.IsNullOrEmpty(tokenType) && !string.IsNullOrEmpty(accessToken))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, accessToken);
            }

            var url = $"{path}?{queryParamsCollection}";
            var ResponseModel = await ExecuteClient(client, url, HttpMethod.Get);
            var answer = await ResponseModel.Content.ReadAsStringAsync();

            return !ResponseModel.IsSuccessStatusCode
                ? GenerateErrorResponse<T>(answer, ResponseModel, $"QueryParams: {queryParamsCollection}")
                : GenerateSuccessResponse<T>(answer);
        });
    }

    public Task<ResponseModel<T>> PostAsyncFormData<T>(string path, Dictionary<string, string> formData,
        string? tokenType = null, string? accessToken = null)
    {
        return ExecuteApiCall(async () =>
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(60);
            if (!string.IsNullOrEmpty(tokenType) && !string.IsNullOrEmpty(accessToken))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, accessToken);
            }

            var url = $"{path}";

            var content = new FormUrlEncodedContent(formData);
            var formDataString = string.Join(" | ", formData.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"));

            var ResponseModel = await ExecuteClient(client, url, HttpMethod.Post, content);

            var answer = await ResponseModel.Content.ReadAsStringAsync();

            return !ResponseModel.IsSuccessStatusCode
                ? GenerateErrorResponse<T>(answer, ResponseModel, $"FormData: {formDataString}")
                : GenerateSuccessResponse<T>(answer);
        });
    }

    public Task<ResponseModel<T>> PostAsyncFormData<T>(string path, Dictionary<string, string> formData, Stream file, string fileName, string contentType, string? tokenType = null, string? accessToken = null)
    {
        return ExecuteApiCall(async () =>
        {
            using var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(60);

            if (!string.IsNullOrEmpty(tokenType) && !string.IsNullOrEmpty(accessToken))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, accessToken);
            }

            using var content = new MultipartFormDataContent();

            var fileContent = new StreamContent(file);
            fileContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            content.Add(fileContent, "file", fileName);

            foreach (var item in formData)
            {
                content.Add(new StringContent(item.Value), item.Key);
            }

            var response = await ExecuteClient(client, path, HttpMethod.Post, content);

            var answer = await response.Content.ReadAsStringAsync();

            return !response.IsSuccessStatusCode
                ? GenerateErrorResponse<T>(
                    answer,
                    response,
                    formData.Any()
                        ? $"FormData: {string.Join(" | ", formData.Select(kvp => $"{Uri.EscapeDataString(kvp.Key)}={Uri.EscapeDataString(kvp.Value)}"))} | file= {fileName}"
                        : $"FormData: file={fileName}"
                    )
                : GenerateSuccessResponse<T>(answer);
        });
    }

    public Task<ResponseModel<T>> PostAsyncJson<T>(string path, Object model,
        string? tokenType = null, string? accessToken = null)
    {
        return ExecuteApiCall(async () =>
        {
            var request = JsonConvert.SerializeObject(model);
            var content = new StringContent(request, Encoding.UTF8, "application/json");
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(300);
            if (!string.IsNullOrEmpty(tokenType) && !string.IsNullOrEmpty(accessToken))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, accessToken);
            }

            var url = $"{path}";
            var ResponseModel = await ExecuteClient(client, url, HttpMethod.Post, content);

            if (typeof(T) == typeof(Stream))
            {
                Stream stream = await ResponseModel.Content.ReadAsStreamAsync();

                return GenerateSuccessResponse<T>((T)(object)stream);
            }
            else
            {
                var answer = await ResponseModel.Content.ReadAsStringAsync();

                return !ResponseModel.IsSuccessStatusCode
                    ? GenerateErrorResponse<T>(answer, ResponseModel, request)
                    : GenerateSuccessResponse<T>(answer);
            }

        });
    }

    public Task<ResponseModel<T>> UploadFileAsync<T>(string path, string filePath,
        string? tokenType = null, string? accessToken = null)
    {
        return ExecuteApiCall(async () =>
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(60);

            if (!string.IsNullOrEmpty(tokenType) && !string.IsNullOrEmpty(accessToken))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, accessToken);
            }

            var url = $"{path}";
            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                using (var content = new MultipartFormDataContent())
                {
                    var streamContent = new StreamContent(fileStream);
                    streamContent.Headers.ContentType = new MediaTypeHeaderValue(GetMimeType(filePath));
                    var getFileName = Path.GetFileName(filePath);

                    content.Add(streamContent, "file", getFileName);

                    var ResponseModel = await ExecuteClient(client, url, HttpMethod.Post, content);
                    var answer = await ResponseModel.Content.ReadAsStringAsync();

                    return !ResponseModel.IsSuccessStatusCode
                        ? GenerateErrorResponse<T>(answer, ResponseModel)
                        : GenerateSuccessResponse<T>(answer);
                }
            }
        });

    }

    public Task<ResponseModel<T>> PutAsync<T>(string path, Object model, string? tokenType = null, string? accessToken = null)
    {
        return ExecuteApiCall(async () =>
        {
            var request = JsonConvert.SerializeObject(model);
            var content = new StringContent(request, Encoding.UTF8, "application/json");
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(60);
            if (!string.IsNullOrEmpty(tokenType) && !string.IsNullOrEmpty(accessToken))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, accessToken);
            }
            var url = $"{path}";
            var ResponseModel = await ExecuteClient(client, url, HttpMethod.Put, content);
            var answer = await ResponseModel.Content.ReadAsStringAsync();

            return !ResponseModel.IsSuccessStatusCode
                ? GenerateErrorResponse<T>(answer, ResponseModel, request)
                : GenerateSuccessResponse<T>(answer);
        });
    }

    public Task<ResponseModel<T>> DeleteAsync<T>(string path,
        string? tokenType = null, string? accessToken = null)
    {
        return ExecuteApiCall(async () =>
        {
            var client = new HttpClient();
            client.Timeout = TimeSpan.FromSeconds(60);
            if (!string.IsNullOrEmpty(tokenType) && !string.IsNullOrEmpty(accessToken))
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(tokenType, accessToken);
            }

            var url = $"{path}";
            var ResponseModel = await ExecuteClient(client, url, HttpMethod.Delete);
            var answer = await ResponseModel.Content.ReadAsStringAsync();

            return !ResponseModel.IsSuccessStatusCode
                ? GenerateErrorResponse<T>(answer, ResponseModel)
                : GenerateSuccessResponse<T>(answer);
        });
    }

    private static string GetMimeType(string filePath)
    {
        var extension = Path.GetExtension(filePath).ToLowerInvariant();

        switch (extension)
        {
            case ".txt": return "text/plain";
            case ".pdf": return "application/pdf";
            case ".doc": return "application/msword";
            case ".docx": return "application/vnd.openxmlformats-officedocument.wordprocessingml.document";
            case ".xls": return "application/vnd.ms-excel";
            case ".xlsx": return "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
            case ".csv": return "text/csv";
            case ".xml": return "text/xml";
            case ".json": return "application/json";
            case ".zip": return "application/zip";
            case ".rar": return "application/x-rar-compressed";
            case ".7z": return "application/x-7z-compressed";
            case ".mp3": return "audio/mpeg";
            case ".wav": return "audio/wav";
            case ".mp4": return "video/mp4";
            case ".mkv": return "video/x-matroska";
            case ".mov": return "video/quicktime";
            case ".avi": return "video/x-msvideo";
            case ".ppt": return "application/vnd.ms-powerpoint";
            case ".pptx": return "application/vnd.openxmlformats-officedocument.presentationml.presentation";
            case ".odt": return "application/vnd.oasis.opendocument.text";
            case ".ods": return "application/vnd.oasis.opendocument.spreadsheet";
            case ".odp": return "application/vnd.oasis.opendocument.presentation";

            case ".png": return "image/png";
            case ".jpg": return "image/jpeg";
            case ".jpeg": return "image/jpeg";
            case ".jfif": return "image/jpeg";
            case ".pjp": return "image/jpeg";
            case ".jpe": return "image/jpeg";
            case ".gif": return "image/gif";
            case ".bmp": return "image/bmp";
            case ".ico": return "image/vnd.microsoft.icon";
            case ".tiff": return "image/tiff";
            case ".svg": return "image/svg+xml";
            case ".webp": return "image/webp";
            case ".svgz": return "image/svg+xml";
            case ".xbm": return "image/x-xbitmap";
            case ".dib": return "image/bmp";
            case ".avif": return "image/avif";
            case ".apng": return "image/apng";
            case ".pjpeg": return "image/pjpeg";
            case ".tif": return "image/tiff";
            case ".jp2": return "image/jp2";
            case ".jpx": return "image/jpx";
            case ".jpm": return "image/jpm";
            case ".mj2": return "image/mj2";
            case ".heif": return "image/heif";
            case ".heic": return "image/heif";
            case ".avci": return "image/avci";
            case ".avcs": return "image/avcs";

            case ".css": return "text/css";
            case ".html": return "text/html";
            case ".htm": return "text/html";
            case ".php": return "application/php";
            case ".js": return "text/javascript";
            case ".go": return "text/x-go";
            case ".py": return "text/x-python";
            case ".java": return "text/x-java-source";
            case ".c": return "text/x-c";
            case ".cpp": return "text/x-c++src";
            case ".cs": return "text/x-csharp";
            case ".swift": return "text/x-swift";
            case ".sh": return "application/x-shellscript";
            case ".sql": return "application/sql";
            case ".rtf": return "application/rtf";
            case ".tar": return "application/x-tar";
            case ".bin": return "application/octet-stream";
            default: return "application/octet-stream"; // Tipo genérico de datos binarios
        }
    }
    private ResponseModel<T> GenerateErrorResponse<T>(string answer, HttpResponseMessage response, string? requestBody = null)
    {
        var errorDetails = new ErrorClientProviderDetails
        {
            IsSuccess = false,
            Message = "Ocurrió un error durante la conexión con la API",
            RequestUrl = response.RequestMessage?.RequestUri?.ToString(),
            RequestMethod = response.RequestMessage?.Method.ToString(),
            RequestBody = requestBody,
            ApiResponse = answer,
            HttpStatusCode = (int)response.StatusCode,
            TimeStamp = DateTime.UtcNow
        };
        try
        {
            var parsedResponse = JsonConvert.DeserializeObject<CodeErrorException>(answer);

            if (parsedResponse is not null)
                errorDetails.Response = parsedResponse;
        }
        catch (Exception)
        {
        }
        var serializedError = JsonConvert.SerializeObject(errorDetails);

        return new ResponseModel<T>
        {
            StatusCode = (int)response.StatusCode,
            IsSuccess = false,
            Message = serializedError
        };
    }

    private ResponseModel<T> GenerateSuccessResponse<T>(object data)
    {
        T obj;
        if (typeof(T) == typeof(Stream))
        {
            obj = (T)data;
        }
        else
        {
            string answer = data as string ?? data?.ToString() ?? string.Empty;

            try
            {
                obj = JsonConvert.DeserializeObject<T>(answer)
                    ?? throw new JsonSerializationException(
                        $"La respuesta no contiene un valor válido para {typeof(T).Name}.");
            }
            catch (JsonReaderException)
            {
                var converted = Convert.ChangeType(
                    answer,
                    Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T));

                obj = converted is T typed
                    ? typed
                    : throw new JsonSerializationException(
                        $"No se pudo convertir la respuesta a {typeof(T).Name}.");
            }
        }

        return new ResponseModel<T>
        {
            IsSuccess = true,
            Result = obj,
            Message = "Petición exitosa"
        };
    }
    private ResponseModel<T> GenerateExceptionResponse<T>(string message)
    {
        return new ResponseModel<T>
        {
            IsSuccess = false,
            Message = "Se ha generado una excepción en el procesamiento de la peteción: " + message,
        };
    }

    private async Task<HttpResponseMessage> ExecuteClient(HttpClient client, string requestUrl, HttpMethod method, HttpContent? content = null)
    {
        HttpResponseMessage response;
        var idRequest = Guid.NewGuid();
        var reloj = new Stopwatch();
        await RegistrarRequestLogInformation(idRequest, requestUrl, method.Method, content);
        reloj.Start();

        switch (method.Method)
        {
            case var metodo when metodo == HttpMethod.Get.Method:
                response = await client.GetAsync(requestUrl);
                break;
            case var metodo when metodo == HttpMethod.Post.Method:
                response = await client.PostAsync(requestUrl, content);
                break;
            case var metodo when metodo == HttpMethod.Put.Method:
                response = await client.PutAsync(requestUrl, content);
                break;
            case var metodo when metodo == HttpMethod.Delete.Method:
                response = await client.DeleteAsync(requestUrl);
                break;
            default:
                throw new InvalidOperationException($"No se ha implementado el método {method.Method} para el llamado de API's externas");
        }

        reloj.Stop();
        await RegistrarResponseLogInformation(idRequest, response, reloj.ElapsedMilliseconds);

        return response;
    }

    private async Task RegistrarRequestLogInformation(Guid idRequest, string url, string method, HttpContent? body = null)
    {
        var contentString = "";
        if (url.Equals($"{_settings.ApiSeguridad}/api/Auth/Login"))
            return;

        if (body is not null)
            contentString = await body.ReadAsStringAsync();
        //_logger.LogInformation($" || idRequest: {idRequest} || url: {url} ({method}) || body: {contentString.Replace("\\\"", "\"")}");
    }
    private async Task RegistrarResponseLogInformation(Guid idRequest, HttpResponseMessage response, long duracion)
    {
        var responseString = await response.Content.ReadAsStringAsync();
        if (responseString.Contains("accessToken", StringComparison.OrdinalIgnoreCase))
            return;

        //_logger.LogInformation($" || idRequest: {idRequest} || duración: {duracion}ms || response $({response.StatusCode}): {responseString.Replace("\\\"", "\"")}");
    }

}