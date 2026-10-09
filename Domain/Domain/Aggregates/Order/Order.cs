namespace Domain;

public class Order : BaseDomainModel
{
    public Guid IdClient { get; private set; }
    public Guid IdServiceType { get; private set; }
    public Guid IdDispatchType { get; private set; }
    public string Number { get; private set; } = string.Empty;
    public Guid IdOrderStatus { get; private set; }
    public Guid IdOrderSubStatus { get; private set; }
    public int VisitNumber { get; set; } = 0;
    public string ReceivedBy { get; private set; } = string.Empty;
    public string? Comments { get; private set; } = string.Empty;
    public Client? Client { get; private set; } = null;
    public ServiceType? ServiceType { get; private set; } = null;
    public DispatchType? DispatchType { get; private set; } = null;
    public OrderStatus? OrderStatus { get; private set; } = null;
    public OrderSubStatus? OrderSubStatus { get; private set; } = null;
    public List<Notification> Notifications { get; private set; } = [];
    public List<Tracking> Trackings { get; private set; } = [];
    private Order() { }
    private Order(Guid id) : base(id) { }
    private Order(Guid id, Guid idClient, Guid idServiceType, Guid idDispatchType, string number, string receivedBy, string? comments) : base(id)
    {
        IdClient = idClient;
        IdServiceType = idServiceType;
        IdDispatchType = idDispatchType;
        Number = number;
        ReceivedBy = receivedBy;
        Comments = comments;
    }
    public static Order Create(Guid idClient, Guid idServiceType, Guid idDispatchType, string number, Guid idOrderStatus, Guid idOrderSubStatus, string receivedBy, string? comments, int maxVisitsToBeReturned, string trackingNumber, List<TrackingEvidenceViewModel> evidences)
    {
        var order = new Order(Guid.NewGuid(), idClient, idServiceType, idDispatchType, number, receivedBy, comments);
        order.ActualizarEstado(idOrderStatus, idOrderSubStatus, maxVisitsToBeReturned, trackingNumber, evidences);
        return order;
    }
    public void ActualizarEstado(Guid idOrderStatus, Guid idOrderSubStatus, int maxVisitsToBeReturned, string trackingNumber, List<TrackingEvidenceViewModel> evidences)
    {
        if (IdOrderStatus == OrderStatusEnum.Delivered.GetId())
            throw new InvalidElementException("El pedido ya fue entregado al consignatario");
        else if (IdOrderStatus == OrderStatusEnum.Returned.GetId())
            throw new InvalidElementException("El pedido ya fue devuelto al cliente");

        IdOrderStatus = idOrderStatus;
        IdOrderSubStatus = idOrderSubStatus;

        var orderStatusEnum = EnumExtensions.GetEnumValueFromId<OrderStatusEnum>(idOrderStatus);
        if (orderStatusEnum == OrderStatusEnum.Delivered || orderStatusEnum == OrderStatusEnum.NotDelivered)
        {
            VisitNumber++;

            if (VisitNumber == maxVisitsToBeReturned)
                RaiseDomainEvents(new ReturnOrderDomainEvent(Number, trackingNumber, OrderSubStatusEnum.WrongAddress));
        }

        if (evidences.Count > 0 && Helper.OrdersStatusForUploadEvidence.Contains(orderStatusEnum))
            RaiseDomainEvents(new UploadTrackingEvidenceDomainEvent(Number, trackingNumber, orderStatusEnum, evidences));

        RaiseDomainEvents(new NotifyChangedOrderStatusDomainEvent(Number, orderStatusEnum));
    }
}