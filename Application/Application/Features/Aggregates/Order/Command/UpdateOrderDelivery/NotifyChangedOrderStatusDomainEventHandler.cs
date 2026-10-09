namespace Application;

using System.Net.Http;
using System.Text;
using System.Text.Json;
using Domain;
using MediatR;
public class NotifyChangedOrderStatusDomainEventHandler(INotificationProfileRepository notificationProfileRepository,
                                                        IHttpClientFactory httpClientFactory,
                                                        IUnitOfWork unitOfWork,
                                                        IMediator mediator) : INotificationHandler<NotifyChangedOrderStatusDomainEvent>
{
    public async Task Handle(NotifyChangedOrderStatusDomainEvent notification, CancellationToken cancellationToken)
    {
        var (idOrder, payload, endpoint) = await GetDataToNotifyByOrderNumber(notification.OrderNumber, notification.OrderStatus);

        await SendNotification(endpoint, payload);

        var newOrderNotification = Notification.Create(idOrder, payload);
        unitOfWork.Repository<Notification>().AddEntity(newOrderNotification);

        await unitOfWork.Complete();
    }
    private async Task<(Guid IdOrder, string Payload, string EndpointNotification)> GetDataToNotifyByOrderNumber(string orderNumber, OrderStatusEnum orderStatusEnum)
    {
        var result = await notificationProfileRepository.GetFormatJsonForNotificationByOrderNumber(orderNumber);

        var idOrder = result.IdOrder;
        var formatJson = result.FormatJson;
        var endpointNotification = result.EndpointNotification;

        if (idOrder == Guid.Empty) throw new NotFoundException($"No se encontró el formato de notificación para la orden {orderNumber}.");
        if (string.IsNullOrWhiteSpace(formatJson)) throw new InvalidElementException("El campo formatJson no debe ser nulo ni vacío.");
        if (string.IsNullOrWhiteSpace(endpointNotification)) throw new InvalidElementException("El endpoint para notificar al cliente no debe ser nulo ni vacío.");

        var idOrderStatus = orderStatusEnum.GetId();
        var orderStatus = (await mediator.Send(new GetOrderStatusQuery())).Where(x => x.Id == idOrderStatus).FirstOrDefault() ?? throw new NotFoundException("El status enviado no es válido");

        var payloadJson = formatJson
            .Replace("{{OrderNumber}}", JsonSerializer.Serialize(orderNumber), StringComparison.Ordinal)
            .Replace("{{OrderStatus}}", JsonSerializer.Serialize(orderStatus.Description), StringComparison.Ordinal);

        return (idOrder, payloadJson, endpointNotification);
    }
    private async Task SendNotification(string endpoint, string payloadJson)
    {
        using var parsedPayload = JsonDocument.Parse(payloadJson);

        using var content = new StringContent(payloadJson, Encoding.UTF8, "application/json");

        var httpClient = httpClientFactory.CreateClient();

        HttpResponseMessage response;
        try
        {
            response = await httpClient.PostAsync(endpoint, content);
        }
        catch (InvalidOperationException ex)
        {
            throw new InvalidElementException(ex.Message);
        }
        catch (HttpRequestException ex)
        {
            throw new InvalidElementException($"No se pudo conectar con el endpoint del cliente: {ex.Message}");
        }

        using (response)
        {
            if (!response.IsSuccessStatusCode)
            {
                throw new ExternalServiceException(
                    $"Falló la notificación al cliente. HTTP {(int)response.StatusCode}.");
            }
        }
    }
}