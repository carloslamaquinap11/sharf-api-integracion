namespace Domain;

public sealed record NotifyChangedOrderStatusDomainEvent(string OrderNumber, OrderStatusEnum OrderStatus) : IDomainEvent;