namespace Application;

using MediatR;
public sealed record CreateOrderDeliveryCommand(TMSDeliveryEventRequest Payload) : IRequest<bool>;