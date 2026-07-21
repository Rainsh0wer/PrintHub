using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrintHub.Domain.Entities;

namespace PrintHub.Infrastructure.Persistence.Configurations;

public class WalletTransactionConfiguration : IEntityTypeConfiguration<WalletTransaction>
{
    public void Configure(EntityTypeBuilder<WalletTransaction> b)
    {
        b.ToTable("WalletTransactions");
        b.HasKey(x => x.Id);

        b.Property(x => x.Amount).HasPrecision(18, 2);
        b.Property(x => x.BalanceAfter).HasPrecision(18, 2);
        b.Property(x => x.RefCode).HasMaxLength(50).IsRequired();
        b.Property(x => x.Description).HasMaxLength(300);
        b.Property(x => x.BankReference).HasMaxLength(100);

        b.HasIndex(x => x.RefCode).IsUnique();

        b.HasOne(x => x.User)
            .WithMany(u => u.WalletTransactions)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Order)
            .WithMany(o => o.WalletTransactions)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class VoucherConfiguration : IEntityTypeConfiguration<Voucher>
{
    public void Configure(EntityTypeBuilder<Voucher> b)
    {
        b.ToTable("Vouchers");
        b.HasKey(x => x.Id);

        b.Property(x => x.Code).HasMaxLength(50).IsRequired();
        b.Property(x => x.Name).HasMaxLength(150);
        b.Property(x => x.Description).HasMaxLength(300);
        b.Property(x => x.DiscountValue).HasPrecision(18, 2);
        b.Property(x => x.MinOrderAmount).HasPrecision(18, 2);
        b.Property(x => x.MaxDiscountAmount).HasPrecision(18, 2);

        b.HasIndex(x => x.Code).IsUnique();
    }
}

public class ReviewConfiguration : IEntityTypeConfiguration<Review>
{
    public void Configure(EntityTypeBuilder<Review> b)
    {
        b.ToTable("Reviews");
        b.HasKey(x => x.Id);

        b.Property(x => x.Comment).HasMaxLength(2000);
        b.Property(x => x.ShopReply).HasMaxLength(2000);

        // One review per order.
        b.HasIndex(x => x.OrderId).IsUnique();

        b.HasOne(x => x.Order)
            .WithOne(o => o.Review)
            .HasForeignKey<Review>(x => x.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Customer)
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Shop)
            .WithMany(s => s.Reviews)
            .HasForeignKey(x => x.ShopId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class ComplaintConfiguration : IEntityTypeConfiguration<Complaint>
{
    public void Configure(EntityTypeBuilder<Complaint> b)
    {
        b.ToTable("Complaints");
        b.HasKey(x => x.Id);

        b.Property(x => x.Description).HasMaxLength(2000).IsRequired();
        b.Property(x => x.ShopResponse).HasMaxLength(2000);
        b.Property(x => x.AdminRuling).HasMaxLength(2000);
        b.Property(x => x.AttachmentUrls).HasMaxLength(1000);
        b.Property(x => x.RefundAmount).HasPrecision(18, 2);

        b.HasOne(x => x.Order)
            .WithMany(o => o.Complaints)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Customer)
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Shop)
            .WithMany()
            .HasForeignKey(x => x.ShopId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.ReplacementOrder)
            .WithMany()
            .HasForeignKey(x => x.ReplacementOrderId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class FavouriteConfiguration : IEntityTypeConfiguration<Favourite>
{
    public void Configure(EntityTypeBuilder<Favourite> b)
    {
        b.ToTable("Favourites");
        b.HasKey(x => x.Id);

        b.Property(x => x.Note).HasMaxLength(200);

        // A customer favourites a shop at most once.
        b.HasIndex(x => new { x.CustomerId, x.ShopId }).IsUnique();

        b.HasOne(x => x.Customer)
            .WithMany(u => u.Favourites)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Shop)
            .WithMany()
            .HasForeignKey(x => x.ShopId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class NotificationConfiguration : IEntityTypeConfiguration<Notification>
{
    public void Configure(EntityTypeBuilder<Notification> b)
    {
        b.ToTable("Notifications");
        b.HasKey(x => x.Id);

        b.Property(x => x.Title).HasMaxLength(200).IsRequired();
        b.Property(x => x.Content).HasMaxLength(1000).IsRequired();
        b.Property(x => x.LinkUrl).HasMaxLength(500);

        b.HasIndex(x => new { x.UserId, x.IsRead });

        b.HasOne(x => x.User)
            .WithMany(u => u.Notifications)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public class AuditLogConfiguration : IEntityTypeConfiguration<AuditLog>
{
    public void Configure(EntityTypeBuilder<AuditLog> b)
    {
        b.ToTable("AuditLogs");
        b.HasKey(x => x.Id);

        b.Property(x => x.Action).HasMaxLength(100).IsRequired();
        b.Property(x => x.EntityName).HasMaxLength(100).IsRequired();
        b.Property(x => x.EntityId).HasMaxLength(50);
        b.Property(x => x.IpAddress).HasMaxLength(50);
        b.Property(x => x.UserAgent).HasMaxLength(300);
        // OldValue / NewValue intentionally unbounded (nvarchar(max)).

        b.HasIndex(x => x.CreatedAt);

        b.HasOne(x => x.ActorUser)
            .WithMany()
            .HasForeignKey(x => x.ActorUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
