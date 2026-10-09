namespace Domain;

public class ServiceType : BaseDomainModel
{
    public string Description { get; private set; } = string.Empty;
    public string IdProvider { get; private set; } = string.Empty;
    public List<Order> Orders { get; private set; } = [];
    private ServiceType() { }
    private ServiceType(Guid id) : base(id) { }
    private ServiceType(Guid id, string description, string idProvider) : base(id)
    {
        Description = description;
        IdProvider = idProvider;
    }
    public static ServiceType Create(string description, string idProvider)
    {
        return new ServiceType(Guid.NewGuid(), description, idProvider);
    }
    public static ServiceType Create(Guid id, string description, string idProvider)
    {
        return new ServiceType(id, description, idProvider);
    }
}