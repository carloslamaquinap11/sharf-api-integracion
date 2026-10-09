namespace Application;

using MediatR;
public sealed record GetEventHistoryByOrderNumberQuery(string OrderNumber) : IRequest<IEnumerable<OrderEventViewModel>>;