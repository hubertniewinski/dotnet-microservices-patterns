namespace WalletService.API.Requests;

public record SeedWalletRequest(Guid UserId, decimal InitialBalance);