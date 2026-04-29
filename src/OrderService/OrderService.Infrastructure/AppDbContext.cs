using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Aggregates;
using OrderService.Infrastructure.Outbox;

namespace OrderService.Infrastructure;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options) { }

    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OutboxMessage> OutboxMessages => Set<OutboxMessage>();

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<Order>().HasKey(o => o.Id);

        builder.Entity<Order>().Property(o => o.UserId)
            .IsRequired();

        builder.Entity<Order>().Property(o => o.Amount)
            .HasColumnType("decimal(18,2)")
            .IsRequired();

        builder.Entity<Order>().Property(o => o.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.Entity<Order>().Ignore(o => o.DomainEvents);
    }
}