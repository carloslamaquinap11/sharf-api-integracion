namespace Domain;

public class NotFoundException : ApplicationException
{
    public NotFoundException(string message) : base(message)
    {
    }
}
