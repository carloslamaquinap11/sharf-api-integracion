namespace Application;

public interface IEmailService
{
    public Task<bool> NotificacionErrorOutboxMessage(Guid id, string type, string mensajeError);
}
