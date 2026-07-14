using Domain.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Repository;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
    : IdentityDbContext<ConcertApplicationUser>(options)
{
    public DbSet<Artist> Artists { get; set; }
    public DbSet<Venue> Venues { get; set; }
    public DbSet<Concert> Concerts { get; set; }
    public DbSet<TicketCategory> TicketCategories { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<Performance> Performances { get; set; }
    public DbSet<ApiClient> ApiClients { get; set; }
    public DbSet<EtlSyncLog> EtlSyncLogs { get; set; }
    public DbSet<InboundEventEntry> InboundEventEntries { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<Ticket>()
            .Property(t => t.Price)
            .HasColumnType("decimal(18,2)");

        builder.Entity<Concert>()
            .Property(c => c.BasePrice)
            .HasColumnType("decimal(18,2)");

        builder.Entity<TicketCategory>()
            .Property(c => c.PriceMultiplier)
            .HasColumnType("decimal(18,4)");

        builder.Entity<Ticket>()
            .HasOne(t => t.Concert)
            .WithMany(c => c.Tickets)
            .HasForeignKey(t => t.ConcertId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Ticket>()
            .HasOne(t => t.TicketCategory)
            .WithMany(c => c.Tickets)
            .HasForeignKey(t => t.TicketCategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
