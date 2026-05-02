using WalletService.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using WalletService.Infrastructure.Persistence.Idempotency;

namespace WalletService.Infrastructure.Repositories;

public class IdempotencyRepository : IIdempotencyRepository
{
    private readonly AppDbContext _db;

    public IdempotencyRepository(AppDbContext db) => _db = db;

    public async Task<string?> GetAsync(Guid idempotencyKey, CancellationToken ct)
    {
        var record = await _db.IdempotencyRecords.FirstOrDefaultAsync(r => r.IdempotencyKey == idempotencyKey, ct);
        return record?.Result;
    }

    public async Task StoreAsync(Guid idempotencyKey, string result, CancellationToken ct)
    {
        await _db.IdempotencyRecords.AddAsync(new IdempotencyRecord
        {
            IdempotencyKey = idempotencyKey,
            Result = result
        }, ct);

        await _db.SaveChangesAsync(ct);
    }
}