namespace Application;

using MediatR;
public sealed record GetEventHistoryQuery() : IRequest<IEnumerable<OrderEventViewModel>>;