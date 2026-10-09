namespace Application;

using MediatR;
public sealed record GetDispatchTypeQuery() : IRequest<IEnumerable<DispatchTypeViewModel>>;