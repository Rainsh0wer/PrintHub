using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using PrintHub.Domain.Common;
using PrintHub.Domain.Entities;

namespace PrintHub.Infrastructure.Persistence;

/// <summary>
/// The single EF Core DbContext for PrintHub. The schema is generated entirely
/// code-first: entity shape comes from the classes, and relationships, indexes,
/// constraints, and precision come from the IEntityTypeConfiguration classes in
/// the Configurations folder, applied via ApplyConfigurationsFromAssembly.
/// </summary>
public class PrintHubDbContext : DbContext
{
    public PrintHubDbContext(DbContextOptions<PrintHubDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    public DbSet<Shop> Shops => Set<Shop>();
    public DbSet<ShopStaff> ShopStaff => Set<ShopStaff>();
    public DbSet<ServiceType> ServiceTypes => Set<ServiceType>();
    public DbSet<ShopService> ShopServices => Set<ShopService>();
    public DbSet<PriceRule> PriceRules => Set<PriceRule>();
    public DbSet<Machine> Machines => Set<Machine>();
    public DbSet<Material> Materials => Set<Material>();
    public DbSet<DocumentFile> DocumentFiles => Set<DocumentFile>();
    public DbSet<Quote> Quotes => Set<Quote>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderItem> OrderItems => Set<OrderItem>();
    public DbSet<OrderStatusHistory> OrderStatusHistories => Set<OrderStatusHistory>();
    public DbSet<WalletTransaction> WalletTransactions => Set<WalletTransaction>();
    public DbSet<Voucher> Vouchers => Set<Voucher>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<Complaint> Complaints => Set<Complaint>();
    public DbSet<Favourite> Favourites => Set<Favourite>();
    public DbSet<Notification> Notifications => Set<Notification>();
    public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PrintHubDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Soft-delete query filters interact with required navigations on entities
        // that are not themselves filtered (e.g. Order -> User). That interaction is
        // intentional and safe here, so the advisory warning is silenced.
        optionsBuilder.ConfigureWarnings(w =>
            w.Ignore(CoreEventId.PossibleIncorrectRequiredNavigationWithQueryFilterInteractionWarning));
    }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        StampAuditTimestamps();
        return base.SaveChangesAsync(cancellationToken);
    }

    public override int SaveChanges()
    {
        StampAuditTimestamps();
        return base.SaveChanges();
    }

    /// <summary>
    /// Centrally stamps CreatedAt/UpdatedAt (UTC) on auditable entities so callers
    /// never set them by hand and the values stay consistent system-wide.
    /// </summary>
    private void StampAuditTimestamps()
    {
        var now = DateTime.UtcNow;
        foreach (var entry in ChangeTracker.Entries<AuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = now;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = now;
                    break;
            }
        }
    }
}
