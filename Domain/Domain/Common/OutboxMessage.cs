namespace Domain;

public sealed class OutboxMessage : BaseDomainModel
{
    public DateTime OccurredOnUtc { get; private set; }
    public string Type { get; private set; } = string.Empty;
    public string Content { get; private set; } = string.Empty;
    public OutboxMessageStatusEnum Status { get; private set; }
    public DateTime? ProcessedOnUtc { get; private set; }
    public string? Error { get; private set; }
    public int RetryCount { get; private set; } = 0;
    public DateTime? NextRetryOnUtc { get; private set; }
    public DateTime? LastAttempOnUtc { get; private set; }

    public OutboxMessage(Guid id, DateTime occurredOnUtc, string type, string content, OutboxMessageStatusEnum status) : base(id)
    {
        OccurredOnUtc = occurredOnUtc;
        Content = content;
        Type = type;
        Status = status;
    }
    public static OutboxMessage Create(DateTime occurredOnUtc, string type, string content, OutboxMessageStatusEnum status)
    {
        return new OutboxMessage(Guid.NewGuid(), occurredOnUtc, type, content, status);
    }
    public void MarkProcessing(DateTime attemptedOnUtc)
    {
        Status = OutboxMessageStatusEnum.Processing;
        LastAttempOnUtc = attemptedOnUtc;
    }

    public void MarkProcessed(DateTime processedOnUtc)
    {
        Status = OutboxMessageStatusEnum.Processed;
        ProcessedOnUtc = processedOnUtc;
        Error = null;
        NextRetryOnUtc = null;
    }

    public void ScheduleRetry(
        int retryCount,
        DateTime nextRetryOnUtc,
        string error)
    {
        Status = OutboxMessageStatusEnum.ScheduledForRetry;
        RetryCount = retryCount;
        NextRetryOnUtc = nextRetryOnUtc;
        Error = error;
    }

    public void MarkDeadLetter(string error, DateTime processedOnUtc)
    {
        Status = OutboxMessageStatusEnum.DeadLetter;
        ProcessedOnUtc = processedOnUtc;
        Error = error;
        NextRetryOnUtc = null;
    }
}