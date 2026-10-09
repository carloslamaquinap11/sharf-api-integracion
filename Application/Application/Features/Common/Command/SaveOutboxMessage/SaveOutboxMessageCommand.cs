namespace Application;

using Domain;
using MediatR;
public sealed record SaveOutboxMessageCommand(IDomainEvent Event, bool ConfirmationRequired = true) : IRequest<bool>;