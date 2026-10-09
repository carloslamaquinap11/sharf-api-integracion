namespace Application;

public sealed class TMSDeliveryEventRequest
{
    public string? ServiceType { get; init; }
    public string? DispatchType { get; init; }
    public string? Status { get; init; }
    public string? SubStatus { get; init; }
    public string? VehicleCode { get; init; }
    public string? CourierName { get; init; }
    public DeliveryDetailsRequest? Details { get; init; }
    public string? EventDate { get; init; }
}

public sealed class DeliveryDetailsRequest
{
    public string? OrderNumber { get; init; }
    public string? TrackingNumber { get; init; }
    public string? ClientCode { get; init; }
    public string? ClientName { get; init; }
    public string? ReceivedBy { get; init; }
    public string? Comments { get; init; }
    public List<EvidenceRequest?>? Evidences { get; init; }
}

public sealed class EvidenceRequest
{
    public string? Label { get; init; }
    public string? FileType { get; init; }
    public string? FileName { get; init; }
    public string? Url { get; init; }
}