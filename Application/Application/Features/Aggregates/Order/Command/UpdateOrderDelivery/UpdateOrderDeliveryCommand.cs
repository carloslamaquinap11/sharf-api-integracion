namespace Application;

using MediatR;
public sealed record UpdateOrderDeliveryCommand(TMSDeliveryEventRequest Payload) : IRequest<bool>;