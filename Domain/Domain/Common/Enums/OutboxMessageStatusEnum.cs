namespace Domain
{
    public enum OutboxMessageStatusEnum
    {
        Pending = 1,
        Processing = 2,
        ScheduledForRetry = 3,
        Processed = 4,
        DeadLetter = 5
    }
}
