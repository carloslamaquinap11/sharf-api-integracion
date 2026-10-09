namespace Application;

using MediatR;
using Domain;
using Newtonsoft.Json;

public sealed class ProcessTMSPayloadDomainEventHandler(IOrderRepository orderRepository, IMediator mediator) : INotificationHandler<ProcessTMSPayloadDomainEvent>
{
    public async Task Handle(ProcessTMSPayloadDomainEvent notification, CancellationToken cancellationToken)
    {
        var payloadRequest = JsonConvert.DeserializeObject<TMSDeliveryEventRequest>(notification.Payload) ?? throw new InvalidElementException("El payload no es válido.");
        var orderNumber = payloadRequest.Details?.OrderNumber ?? throw new InvalidElementException("El detalle enviado es nulo");
        var order = await orderRepository.GetOrderByNumber(orderNumber);

        IRequest<bool> command = order is null ? new CreateOrderDeliveryCommand(payloadRequest) 
                                                : new UpdateOrderDeliveryCommand(payloadRequest);
        
        await mediator.Send(command);
    }
}