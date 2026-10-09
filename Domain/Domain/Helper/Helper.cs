namespace Domain;

public static class Helper
{
    public static readonly IReadOnlySet<OrderStatusEnum> OrdersStatusForUploadEvidence = new HashSet<OrderStatusEnum>
        {
            OrderStatusEnum.Collected,
            OrderStatusEnum.NotCollected,
            OrderStatusEnum.Delivered,
            OrderStatusEnum.NotDelivered,
            OrderStatusEnum.Returned,
            OrderStatusEnum.NotReturned
        };
}