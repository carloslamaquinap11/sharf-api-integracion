namespace Domain;

public class NotificationProfile : BaseDomainModel
{
    public string Description { get; private set; } = string.Empty;
    public string FormatJson { get; private set; } = string.Empty;
    public List<Client> Clients { get; set; } = [];
    private NotificationProfile() { }
    private NotificationProfile(Guid id) : base(id) { }
    private NotificationProfile(Guid id, string description, string formatJson) : base(id)
    {
        Description = description;
        FormatJson = formatJson;
    }
    public static NotificationProfile Create(string description, string formatJson)
    {
        return new NotificationProfile(Guid.NewGuid(), description, formatJson);
    }
    public static NotificationProfile Create(Guid id, string description, string formatJson)
    {
        return new NotificationProfile(id, description, formatJson);
    }
}