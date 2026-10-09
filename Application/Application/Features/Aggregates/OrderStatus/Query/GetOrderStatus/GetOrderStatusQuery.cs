namespace Application;

using MediatR;
public sealed record GetOrderStatusQuery : IRequest<IEnumerable<OrderStatusViewModel>>;