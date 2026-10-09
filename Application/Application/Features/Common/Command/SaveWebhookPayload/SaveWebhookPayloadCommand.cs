namespace Application;

using MediatR;
using Domain;
public sealed class SaveWebhookPayloadCommandHandler(IUnitOfWork unitOfWork, IMediator mediator) : IRequestHandler<SaveWebhookPayloadCommand, bool>
{
    public async Task<bool> Handle(SaveWebhookPayloadCommand request, CancellationToken cancellationToken)
    {
        var processEvent = new ProcessTMSPayloadDomainEvent(request.Payload);
        await mediator.Send(new SaveOutboxMessageCommand(processEvent, false));

        await unitOfWork.Complete();

        return true;
    }
}