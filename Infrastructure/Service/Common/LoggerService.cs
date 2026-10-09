namespace Service;

using Application;
using Microsoft.Extensions.Hosting;
using System.Text.Json;

public class LoggerService(IHostEnvironment hostEnvironment) : ILoggerService
{
    private readonly string _logDirectory =
        Path.Combine(hostEnvironment.ContentRootPath, "logs");

    private static readonly SemaphoreSlim FileLock = new(1, 1);

    public Task LogDebug(string message) => WriteLogAsync("DEBUG", message);

    public Task LogError(string message) => WriteLogAsync("ERROR", message);

    public Task LogInfo(string message) => WriteLogAsync("INFO", message);

    public Task LogWarn(string message) => WriteLogAsync("WARN", message);

    public Task LogInfo(string message, string url, string method, string body) =>
        WriteLogAsync("INFO", message, new
        {
            url,
            method,
            body
        });

    private async Task WriteLogAsync(string level, string message, object? data = null)
    {
        var now = DateTimeOffset.UtcNow;
        var hourlyDirectory = Path.Combine(_logDirectory, now.ToString("yyyy-MM-dd"));
        var filePath = Path.Combine(hourlyDirectory, $"{now:HH}.log");

        Directory.CreateDirectory(hourlyDirectory);

        var entry = new
        {
            timestamp = now,
            environment = hostEnvironment.EnvironmentName,
            level,
            message,
            data
        };

        var line = JsonSerializer.Serialize(entry) + Environment.NewLine;

        await FileLock.WaitAsync();
        try
        {
            await File.AppendAllTextAsync(filePath, line);
        }
        finally
        {
            FileLock.Release();
        }
    }
}