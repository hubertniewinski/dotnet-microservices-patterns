namespace WalletService.API.Requests;

public record DebitWalletRequest(Guid UserId, Guid OrderId, decimal Amount);