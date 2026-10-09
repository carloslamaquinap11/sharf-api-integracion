namespace Domain;

public class Notification : BaseDomainModel
{
    public Guid IdOrder { get; private set; }
    public string Content { get; private set; } = string.Empty;
    public Order? Order { get; set; } = null;
    private Notification() { }
    private Notification(Guid id) : base(id) { }
    private Notification(Guid id, Guid idOrder, string content) : base(id)
    {
        IdOrder = idOrder;
        Content = content;
    }
    public static Notification Create(Guid idOrder, string content)
    {
        return new Notification(Guid.NewGuid(), idOrder, content);
    }
}