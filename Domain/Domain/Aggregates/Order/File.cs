namespace Domain;

public class File : BaseDomainModel
{
    public Guid IdTracking { get; private set; }
    public Guid IdOrderStatus { get; private set; }
    public string Label { get; private set; } = string.Empty;
    public string FileType { get; private set; } = string.Empty;
    public string PathUrl { get; private set; } = string.Empty;
    public Tracking? Tracking { get; private set; } = null;
    public OrderStatus? OrderStatus { get; private set; } = null;
    private File() { }
    private File(Guid id) : base(id) { }
    private File(Guid id, Guid idTracking, Guid idOrderStatus, string label, string fileType, string pathUrl) : base(id)
    {
        IdTracking = idTracking;
        IdOrderStatus = idOrderStatus;
        Label = label;
        FileType = fileType;
        PathUrl = pathUrl;
    }
    public static File Create(Guid idTracking, Guid idOrderStatus, string label, string fileType, string pathUrl)
    {
        return new File(Guid.NewGuid(), idTracking, idOrderStatus, label, fileType, pathUrl);
    }
}