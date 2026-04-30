using WalletService.Domain.Aggregates;
using WalletService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using WalletService.Infrastructure.Outbox;
using Newtonsoft.Json;

namespace WalletService.Infrastructure.Repositories;

public class WalletRepository : IWalletRepository
{
    private readonly AppDbContext _db;

    public WalletRepository(AppDbContext db) => _db = db;

    public async Task<Wallet?> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        => await _db.Wallets.FirstOrDefaultAsync(w => w.UserId == userId, ct);

    public async Task SaveAsync(Wallet wallet, CancellationToken ct = default)
    {
        _db.Wallets.Update(wallet);

        foreach (var evt in wallet.DomainEvents)
        {
            await _db.OutboxMessages.AddAsync(new OutboxMessage
            {
                EventType = evt.GetType().Name,
                Payload = JsonConvert.SerializeObject(evt, new JsonSerializerSettings
                {
                    TypeNameHandling = TypeNameHandling.All
                })
            }, ct);
        }

        wallet.ClearDomainEvents();

        await _db.SaveChangesAsync(ct);
    }
}