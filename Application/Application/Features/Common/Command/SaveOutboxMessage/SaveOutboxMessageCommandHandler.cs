namespace Application;

using Domain;
using MediatR;
using Newtonsoft.Json;
public sealed class SaveOutboxMessageCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<SaveOutboxMessageCommand, bool>
{
    public async Task<bool> Handle(SaveOutboxMessageCommand request, CancellationToken cancellationToken)
    {
        var outbox = OutboxMessage.Create(DateTime.UtcNow, request.Event.GetType().Name, JsonConvert.SerializeObject(request.Event), OutboxMessageStatusEnum.Pending);
        unitOfWork.Repository<OutboxMessage>().AddEntity(outbox);
        
        if (request.ConfirmationRequired)
            await unitOfWork.Complete();

        return true;
    }
}