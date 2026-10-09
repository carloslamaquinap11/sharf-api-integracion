namespace Domain;

public class OrderStatus : BaseDomainModel
{
    public string Description { get; private set; } = string.Empty;
    public string IdProvider { get; private set; } = string.Empty;
    public List<Order> Orders { get; private set; } = [];
    public List<File> Files { get; private set; } = [];
    private OrderStatus() { }
    private OrderStatus(Guid id) : base(id) { }
    private OrderStatus(Guid id, string description, string idProvider) : base(id)
    {
        Description = description;
        IdProvider = idProvider;
    }
    public static OrderStatus Create(string description, string idProvider)
    {
        return new OrderStatus(Guid.NewGuid(), description, idProvider);
    }
    public static OrderStatus Create(Guid id, string description, string idProvider)
    {
        return new OrderStatus(id, description, idProvider);
    }
}