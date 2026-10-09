namespace Application;

public class OrderEventViewModel
{
    public string Event { get; set; } = string.Empty;
    public string Payload { get; set; } = string.Empty;
    public DateTime CreatedDate { get; set; }
    public string? Error { get; set; }
}