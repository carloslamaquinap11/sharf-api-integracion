namespace Application;

using MediatR;
public sealed record GetServiceTypeQuery() : IRequest<IEnumerable<ServiceTypeViewModel>>;