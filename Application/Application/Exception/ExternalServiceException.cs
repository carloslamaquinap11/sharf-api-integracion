namespace Application;

public class ExternalServiceException : ApplicationException
{
    public ExternalServiceException(string mensaje) : base(mensaje) { }
}