namespace Shared.Contracts.Events;

public record WalletDebitedEvent(Guid OrderId, Guid UserId, decimal Amount);