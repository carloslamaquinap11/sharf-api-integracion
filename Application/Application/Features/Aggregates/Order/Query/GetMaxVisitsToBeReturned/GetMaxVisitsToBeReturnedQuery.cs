namespace Application;

using MediatR;
public sealed record GetMaxVisitsToBeReturnedQuery : IRequest<int>;