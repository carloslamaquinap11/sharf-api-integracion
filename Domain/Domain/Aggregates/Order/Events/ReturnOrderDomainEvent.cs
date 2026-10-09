namespace Domain;

public sealed record ReturnOrderDomainEvent(string OrderNumber, string TrackingNumber, OrderSubStatusEnum OrderSubStatus) : IDomainEvent;