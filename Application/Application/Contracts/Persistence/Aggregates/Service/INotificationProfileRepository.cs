using Domain;

namespace Application;

public interface INotificationProfileRepository : IRepositoryBase<NotificationProfile>
{
    Task<(Guid IdOrder, string? FormatJson, string? EndpointNotification)> GetFormatJsonForNotificationByOrderNumber(string orderNumber);
}
