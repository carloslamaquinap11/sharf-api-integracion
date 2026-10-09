namespace Application;

public class OrderStatusViewModel
{
    public Guid Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public string IdProvider { get; set; } = string.Empty;
}