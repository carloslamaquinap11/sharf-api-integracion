namespace Service;

public class LoginViewModel
{
    public string AccessToken { get; set; } = string.Empty;
    public bool IsAuthenticated { get; set; } = false;
}