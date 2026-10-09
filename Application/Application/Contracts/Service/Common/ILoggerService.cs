namespace Application;

public interface ILoggerService
{
    Task LogDebug(string message);
    Task LogError(string message);
    Task LogInfo(string message);
    Task LogInfo(string message, string url, string method, string body);
    Task LogWarn(string message);
}