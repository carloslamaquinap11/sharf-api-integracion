namespace Domain;

public sealed record TrackingEvidenceViewModel(
    string Label,
    string FileType,
    string FileName,
    string Url);
public sealed record UploadTrackingEvidenceDomainEvent(string OrderNumber, string TrackingNumber, OrderStatusEnum OrderStatus, List<TrackingEvidenceViewModel> Evidences) : IDomainEvent;