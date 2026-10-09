namespace Persistence;

using Application;
using Domain;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

public class NotificationProfileRepository : RepositoryBase<NotificationProfile>, INotificationProfileRepository
{
    public NotificationProfileRepository(ApplicationDbContext context) : base(context) { }
    public async Task<(Guid IdOrder, string? FormatJson, string? EndpointNotification)> GetFormatJsonForNotificationByOrderNumber(string orderNumber)
    {
    var result = await context.Order
        .AsNoTracking()
        .Where(order => order.Number == orderNumber && order.IsActive)
        .Select(order => new
        {
            IdOrder = order.Id,
            FormatJson = order.Client == null ||
                         order.Client.NotificationProfile == null
                ? null
                : order.Client.NotificationProfile.FormatJson,

            EndpointNotification = order.Client == null
                ? null
                : order.Client.EndpointNotification
        })
        .FirstOrDefaultAsync();

    return result is null
        ? (Guid.Empty, null, null)
        : (result.IdOrder, result.FormatJson, result.EndpointNotification);
}
}
