using WalletService.Domain.Aggregates;

namespace WalletService.Domain.Interfaces;

public interface IWalletRepository
{
    Task<Wallet?> GetByUserIdAsync(Guid userId, CancellationToken ct = default);
    Task SaveAsync(Wallet wallet, bool isNew = false, CancellationToken ct = default);
}