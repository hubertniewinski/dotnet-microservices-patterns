namespace WalletService.Application.Interfaces;

public interface IIdempotencyRepository
{
    Task<string?> GetAsync(Guid idempotencyKey, CancellationToken ct = default);
    Task StoreAsync(Guid idempotencyKey, string result, CancellationToken ct = default);
}