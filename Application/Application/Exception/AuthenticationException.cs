namespace Application;

public class AuthenticationException : ApplicationException
{
    public AuthenticationException(string message) : base(message) { }
}