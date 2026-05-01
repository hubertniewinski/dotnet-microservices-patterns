namespace Shared.Contracts.Events;

public record WalletDebitFailedEvent(Guid OrderId, Guid UserId, decimal Amount);