using WalletService.Domain.Aggregates;
using WalletService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;
using WalletService.Infrastructure.Outbox;
using Newtonsoft.Json;
using WalletService.Domain.Events;
using Shared.Contracts.Events;

namespace WalletService.Infrastructure.Repositories;

public class WalletRepository : IWalletRepository
{
    private readonly AppDbContext _db;

    public WalletRepository(AppDbContext db) => _db = db;

    public async Task<Wallet?> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        => await _db.Wallets.FirstOrDefaultAsync(w => w.UserId == userId, ct);

    public async Task SaveAsync(Wallet wallet, bool isNew = false, CancellationToken ct = default)
    {
        if (isNew)
        {
            _db.Wallets.Add(wallet);
        }
        else
        {
            _db.Wallets.Update(wallet);
        }

        foreach (var evt in wallet.DomainEvents)
        {
            var contractEvent = evt switch
            {
                WalletDebited e => (object)new WalletDebitedEvent(e.OrderId, e.UserId, e.Amount),
                WalletDebitFailed e => (object)new WalletDebitFailedEvent(e.OrderId, e.UserId, e.Amount),
                _ => null
            };

            if (contractEvent is null)
            {
                continue;
            }

            await _db.OutboxMessages.AddAsync(new OutboxMessage
            {
                EventType = contractEvent.GetType().AssemblyQualifiedName!,
                Payload = JsonConvert.SerializeObject(contractEvent)
            }, ct);
        }

        wallet.ClearDomainEvents();

        await _db.SaveChangesAsync(ct);
    }
}