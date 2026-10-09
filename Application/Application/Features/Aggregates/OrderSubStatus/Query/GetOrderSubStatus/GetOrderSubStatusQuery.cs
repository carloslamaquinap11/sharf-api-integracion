namespace Application;

using MediatR;
public sealed record GetOrderSubStatusQuery : IRequest<IEnumerable<OrderSubStatusViewModel>>;