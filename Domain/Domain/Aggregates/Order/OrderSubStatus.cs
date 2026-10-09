namespace Domain;

public class OrderSubStatus : BaseDomainModel
{
    public string Description { get; private set; } = string.Empty;
    public string IdProvider { get; private set; } = string.Empty;
    public List<Order> Orders { get; private set; } = [];
    private OrderSubStatus() { }
    private OrderSubStatus(Guid id) : base(id) { }
    private OrderSubStatus(Guid id, string description, string idProvider) : base(id)
    {
        Description = description;
        IdProvider = idProvider;
    }
    public static OrderSubStatus Create(string description, string idProvider)
    {
        return new OrderSubStatus(Guid.NewGuid(), description, idProvider);
    }
    public static OrderSubStatus Create(Guid id, string description, string idProvider)
    {
        return new OrderSubStatus(id, description, idProvider);
    }
}