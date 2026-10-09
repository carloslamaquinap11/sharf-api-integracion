namespace Application;

using MediatR;
public sealed record SaveWebhookPayloadCommand(string Payload) : IRequest<bool>;