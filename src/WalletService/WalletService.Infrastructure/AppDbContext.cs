using Microsoft.EntityFrameworkCore;
using WalletService.Domain.Aggregates;
using WalletService.Infrastructure.Outbox;
using WalletService.Infrastructure.Persistence.Idempotency;

namespace WalletService.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Wallet> Wallets => Set<Wallet>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();
    public DbSet<IdempotencyRecord> IdempotencyRecords => Set<IdempotencyRecord>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Wallet>().HasKey(o => o.Id);
        builder.Entity<Wallet>().Property(o => o.UserId)
            .IsRequired();
        builder.Entity<Wallet>().Property(o => o.Balance)
            .HasColumnType("decimal(18,2)")
            .IsRequired();
        builder.Entity<Wallet>().Ignore(o => o.DomainEvents);


        builder.Entity<IdempotencyRecord>().HasKey(i => i.IdempotencyKey);
        builder.Entity<IdempotencyRecord>().Property(i => i.Result)
            .IsRequired();
        builder.Entity<IdempotencyRecord>().Property(i => i.CreatedAt)
            .IsRequired();
    }
}