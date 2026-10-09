namespace Application;

public class IntegrationException : ApplicationException
{
    public int StatusCode { get; set; }
    public IntegrationException(int statusCode, string message) : base(message)
    {
        StatusCode = statusCode;
    }
}