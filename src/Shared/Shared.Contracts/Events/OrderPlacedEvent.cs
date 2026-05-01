namespace Shared.Contracts.Events;

public record OrderPlacedEvent(Guid OrderId, Guid UserId, decimal Amount);