namespace Application;

public interface ICurrentUserService
{
    Guid? UserId { get; }
    Guid? AplicacionId { get; }
    string Email { get; }
    string DocumentNumber { get; }
}