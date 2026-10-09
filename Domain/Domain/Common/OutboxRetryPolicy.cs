namespace Domain;

public sealed class OutboxRetryPolicy : BaseDomainModel
{
    public const string DefaultEventType = "DEFAULT";

    public string EventType { get; private set; } = string.Empty;
    public int MaxRetries { get; private set; }
    public int InitialDelaySeconds { get; private set; }
    public BackoffStrategyEnum Strategy { get; private set; }
    public int MaxDelaySeconds { get; private set; }
    public int ZombiRecoveryMinutes { get; private set; }
    public bool RecuperarZombisComoRetry { get; private set; }

    private OutboxRetryPolicy() { }

    private OutboxRetryPolicy(
        Guid id,
        string eventType,
        int maxRetries,
        int initialDelaySeconds,
        BackoffStrategyEnum strategy,
        int maxDelaySeconds,
        int zombiRecoveryMinutes,
        bool recuperarZombisComoRetry) : base(id)
    {
        EventType = eventType;
        MaxRetries = maxRetries;
        InitialDelaySeconds = initialDelaySeconds;
        Strategy = strategy;
        MaxDelaySeconds = maxDelaySeconds;
        ZombiRecoveryMinutes = zombiRecoveryMinutes;
        RecuperarZombisComoRetry = recuperarZombisComoRetry;
    }

    public static OutboxRetryPolicy Create(
        string eventType,
        int maxRetries,
        int initialDelaySeconds,
        BackoffStrategyEnum strategy,
        int maxDelaySeconds,
        int zombiRecoveryMinutes,
        bool recuperarZombisComoRetry = false)
    {
        return new OutboxRetryPolicy(
            Guid.NewGuid(),
            eventType,
            maxRetries,
            initialDelaySeconds,
            strategy,
            maxDelaySeconds,
            zombiRecoveryMinutes,
            recuperarZombisComoRetry);
    }

    public int CalcularBackoffSegundos(int proximoIntento)
    {
        if (proximoIntento < 1) proximoIntento = 1;

        int baseDelay = Strategy == BackoffStrategyEnum.Exponential
            ? InitialDelaySeconds * (int)Math.Pow(2, proximoIntento - 1)
            : InitialDelaySeconds;

        int jitter = Random.Shared.Next(0, 30);
        int total = baseDelay + jitter;

        return total > MaxDelaySeconds ? MaxDelaySeconds : total;
    }

    public static OutboxRetryPolicy CrearDefaultEnMemoria()
    {
        return new OutboxRetryPolicy(
            Guid.Empty,
            DefaultEventType,
            maxRetries: 0,
            initialDelaySeconds: 60,
            strategy: BackoffStrategyEnum.Exponential,
            maxDelaySeconds: 3600,
            zombiRecoveryMinutes: 30,
            recuperarZombisComoRetry: false);
    }
}