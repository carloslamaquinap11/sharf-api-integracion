namespace Persistence;

using Application;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Quartz;
using System.Reflection;

[DisallowConcurrentExecution]
internal sealed class ProcessOutboxMessagesJob(
    ApplicationDbContext dbContext,
    IMemoryCacheService memoryCacheService,
    IConfigurationRepository configurationRepository,
    ILoggerService loggerService,
    IPublisher publisher,
    IEmailService emailService,
    IOutboxRetryPolicyResolver policyResolver) : IJob
{
    private static readonly JsonSerializerSettings JsonSerializerSettings = new()
    {
        TypeNameHandling = TypeNameHandling.All
    };
    private async Task<bool> IsActiveProcess()
    {
        var key = "API_INTEGRACION_JOB_OUTBOX_ACTIVADO";
        var isJobActive = false;

        var (exists, isJobActiveString) = await memoryCacheService.TryGetValue<string>($"Config:{key}");
        if (!exists)
        {
            isJobActiveString = await configurationRepository.GetValueByKey(key) ?? string.Empty;
            bool.TryParse(isJobActiveString, out var parsedValue);
            isJobActive = parsedValue;

            if (isJobActive)
                await memoryCacheService.SetValue($"Config:{key}", isJobActiveString, 60);
        }
        isJobActive = bool.Parse(isJobActiveString!);
        return isJobActive;
    }
    public async ValueTask Execute(
        IJobExecutionContext context,
        CancellationToken cancellationToken)
    {
        var isActiveProcess = await IsActiveProcess();
        if (!isActiveProcess) return;

        var now = DateTime.UtcNow;

        var messages = await dbContext.OutboxMessage
            .Where(message =>
                message.Status == OutboxMessageStatusEnum.Pending ||
                message.Status == OutboxMessageStatusEnum.ScheduledForRetry)
            .Where(message =>
                message.NextRetryOnUtc == null ||
                message.NextRetryOnUtc <= now)
            .OrderBy(message => message.OccurredOnUtc)
            .Take(10)
            .ToListAsync(cancellationToken);

        foreach (var message in messages)
        {
            cancellationToken.ThrowIfCancellationRequested();

            message.MarkProcessing(DateTime.UtcNow);
            await dbContext.SaveChangesAsync(cancellationToken);

            Exception? publishException = null;

            try
            {
                if (!EventTypes.Value.TryGetValue(message.Type, out var matches) || matches.Length != 1)
                {
                    throw new InvalidOperationException(
                        $"Tipo de evento inexistente o ambiguo: {message.Type}");
                }

                var domainEvent = JsonConvert.DeserializeObject(message.Content, matches[0]) as IDomainEvent
                    ?? throw new InvalidOperationException($"No se pudo deserializar el evento {message.Type}.");

                await publisher.Publish(domainEvent, cancellationToken);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                publishException = exception;
            }

            if (publishException is null)
            {
                message.MarkProcessed(DateTime.UtcNow);
                await dbContext.SaveChangesAsync(cancellationToken);
                await loggerService.LogInfo(
                    $"Outbox {message.Type} ({message.Id}) procesado.");
                continue;
            }

            await HandleFailureAsync(message, publishException, cancellationToken);
        }
    }

    private async Task HandleFailureAsync(
        OutboxMessage message,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var policy = await policyResolver.ResolveAsync(message.Type);
        var nextAttempt = message.RetryCount + 1;
        var isTransient = IsTransientException(exception);

        if (!isTransient || nextAttempt > policy.MaxRetries)
        {
            var error = !isTransient
                ? $"[ERROR_NO_RETRIABLE] {exception}"
                : $"[RETRIES_AGOTADOS] {exception}";

            message.MarkDeadLetter(error, DateTime.UtcNow);
            await dbContext.SaveChangesAsync(cancellationToken);

            await loggerService.LogError(
                $"Outbox {message.Type} ({message.Id}) enviado a DeadLetter.");

            try
            {
                await emailService.NotificacionErrorOutboxMessage(
                    message.Id,
                    message.Type,
                    error);
            }
            catch (Exception emailException)
            {
                await loggerService.LogError(
                    $"Falló la notificación de DeadLetter para {message.Id}: " +
                    emailException.Message);
            }

            return;
        }

        var delaySeconds = policy.CalcularBackoffSegundos(nextAttempt);
        var nextRetry = DateTime.UtcNow.AddSeconds(delaySeconds);

        message.ScheduleRetry(
            nextAttempt,
            nextRetry,
            exception.ToString());

        await dbContext.SaveChangesAsync(cancellationToken);

        await loggerService.LogInfo(
            $"Outbox {message.Type} ({message.Id}) reprogramado para el " +
            $"intento #{nextAttempt} en {delaySeconds} segundos.");
    }

    private static bool IsTransientException(Exception exception)
    {
        // if (exception is HttpRequestException or TimeoutException)   // TODO: Descomentar para salir de pruebas de reintentos
        //     return true;

        // if (exception is TaskCanceledException)
        //     return true;

        // return exception.InnerException is not null &&
        //        IsTransientException(exception.InnerException);
        return true;
    }
    private static readonly Lazy<Dictionary<string, Type[]>> EventTypes = new(() =>
    AppDomain.CurrentDomain
        .GetAssemblies()
        .SelectMany(GetLoadableTypes)
        .Where(type =>
            type.IsClass &&
            !type.IsAbstract &&
            typeof(IDomainEvent).IsAssignableFrom(type))
        .GroupBy(type => type.Name, StringComparer.Ordinal)
        .ToDictionary(
            group => group.Key,
            group => group.ToArray(),
            StringComparer.Ordinal));

    private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException exception)
        {
            return exception.Types.OfType<Type>();
        }
    }
}