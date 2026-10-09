namespace Domain;

public sealed record ProcessTMSPayloadDomainEvent(string Payload) : IDomainEvent;