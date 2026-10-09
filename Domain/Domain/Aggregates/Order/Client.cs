namespace Domain;

public class Client : BaseDomainModel
{
    public string Code { get; private set; } = string.Empty;
    public string Name { get; private set; } = string.Empty;
    public string EndpointNotification { get; private set; } = string.Empty;
    public Guid? IdNotificationProfile { get; private set; }
    public List<Order> Orders { get; private set; } = [];
    public NotificationProfile? NotificationProfile { get; private set; } = null;
    private Client() { }
    private Client(Guid id) : base(id) { }
    private Client(Guid id, string code, string name, Guid? idNotificationProfile, string endpointNotification) : base(id)
    {
        Code = code;
        Name = name;
        IdNotificationProfile = idNotificationProfile;
        EndpointNotification = endpointNotification;
    }
    public static Client Create(string code, string name, Guid? idNotificationProfile, string endpointNotification)
    {
        return new Client(Guid.NewGuid(), code, name, idNotificationProfile, endpointNotification);
    }
}