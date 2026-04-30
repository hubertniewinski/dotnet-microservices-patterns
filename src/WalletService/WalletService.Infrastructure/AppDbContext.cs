using Microsoft.EntityFrameworkCore;
using WalletService.Domain.Aggregates;
using WalletService.Infrastructure.Outbox;

namespace WalletService.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Wallet> Wallets => Set<Wallet>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Wallet>().HasKey(o => o.Id);

        builder.Entity<Wallet>().Property(o => o.UserId)
            .IsRequired();

        builder.Entity<Wallet>().Property(o => o.Balance)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Entity<Wallet>().Ignore(o => o.DomainEvents);
    }
}