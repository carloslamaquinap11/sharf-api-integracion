namespace Application;

using Domain;
using MediatR;
public class ReturnOrderDomainEventHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork, IMediator mediator) : INotificationHandler<ReturnOrderDomainEvent>
{
    public async Task Handle(ReturnOrderDomainEvent notification, CancellationToken cancellationToken)
    {
        var order = await orderRepository.GetOrderByNumber(notification.OrderNumber) ?? throw new NotFoundException($"No se encontró el pedido de número {notification.OrderNumber}");

        var maxVisitsToBeReturned = await mediator.Send(new GetMaxVisitsToBeReturnedQuery());
        order.ActualizarEstado(OrderStatusEnum.ToBeReturn.GetId(), notification.OrderSubStatus.GetId(), maxVisitsToBeReturned, notification.TrackingNumber, []);

        unitOfWork.Repository<Order>().UpdateEntity(order);
        
        await unitOfWork.Complete();
    }
}