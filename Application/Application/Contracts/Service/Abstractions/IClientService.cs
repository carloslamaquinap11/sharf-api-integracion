namespace Application;


public interface IClientService
{
    Task<ResponseModel<T>> GetAsync<T>(string path, string? tokenType = null, string? accessToken = null);
    Task<ResponseModel<T>> GetAsyncWithQueryParams<T>(string path, Dictionary<string, string> queryParams, string? tokenType = null, string? accessToken = null);
    Task<ResponseModel<T>> PostAsyncFormData<T>(string path, Dictionary<string, string> formData, string? tokenType = null, string? accessToken = null);
    Task<ResponseModel<T>> PostAsyncFormData<T>(string path, Dictionary<string, string> formData, Stream file, string fileName, string contentType, string? tokenType = null, string? accessToken = null);
    Task<ResponseModel<T>> PostAsyncJson<T>(string path, Object model, string? tokenType = null, string? accessToken = null);
    Task<ResponseModel<T>> UploadFileAsync<T>(string path, string filePath, string? tokenType = null, string? accessToken = null);
    Task<ResponseModel<T>> PutAsync<T>(string path, Object model, string? tokenType = null, string? accessToken = null);
    Task<ResponseModel<T>> DeleteAsync<T>(string path, string? tokenType = null, string? accessToken = null);
}