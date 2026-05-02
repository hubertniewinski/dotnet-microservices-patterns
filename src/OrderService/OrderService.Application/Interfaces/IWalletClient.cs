namespace OrderService.Application.Interfaces;

public interface IWalletClient
{
    Task<bool> CheckBalanceAsync(Guid UserId, decimal Amount, CancellationToken ct = default);
}