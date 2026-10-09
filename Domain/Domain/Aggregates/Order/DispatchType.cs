namespace Domain;

public class DispatchType : BaseDomainModel
{
    public string Description { get; private set; } = string.Empty;
    public string IdProvider { get; private set; } = string.Empty;
    public List<Order> Orders { get; private set; } = [];
    private DispatchType() { }
    private DispatchType(Guid id) : base(id) { }
    private DispatchType(Guid id, string description, string idProvider) : base(id)
    {
        Description = description;
        IdProvider = idProvider;
    }
    public static DispatchType Create(string description, string idProvider)
    {
        return new DispatchType(Guid.NewGuid(), description, idProvider);
    }
    public static DispatchType Create(Guid id, string description, string idProvider)
    {
        return new DispatchType(id, description, idProvider);
    }
}