namespace WalletService.API.Requests;

public record DebitWalletRequest(Guid IdempotencyKey, Guid UserId, Guid OrderId, decimal Amount);