using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrintHub.Domain.Entities;

namespace PrintHub.Infrastructure.Persistence.Configurations;

public class DocumentFileConfiguration : IEntityTypeConfiguration<DocumentFile>
{
    public void Configure(EntityTypeBuilder<DocumentFile> b)
    {
        b.ToTable("DocumentFiles");
        b.HasKey(x => x.Id);

        b.Property(x => x.FileName).HasMaxLength(260).IsRequired();
        b.Property(x => x.StoragePath).HasMaxLength(500).IsRequired();
        b.Property(x => x.ContentType).HasMaxLength(100).IsRequired();

        b.HasOne(x => x.Owner)
            .WithMany(u => u.Documents)
            .HasForeignKey(x => x.OwnerUserId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class QuoteConfiguration : IEntityTypeConfiguration<Quote>
{
    public void Configure(EntityTypeBuilder<Quote> b)
    {
        b.ToTable("Quotes");
        b.HasKey(x => x.Id);

        b.Property(x => x.SubTotal).HasPrecision(18, 2);
        // BreakdownJson is intentionally unbounded (nvarchar(max)).

        b.HasOne(x => x.Customer)
            .WithMany()
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Shop)
            .WithMany()
            .HasForeignKey(x => x.ShopId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> b)
    {
        b.ToTable("Orders");
        b.HasKey(x => x.Id);

        b.Property(x => x.OrderCode).HasMaxLength(30).IsRequired();
        b.Property(x => x.CustomerNote).HasMaxLength(1000);
        b.Property(x => x.DeliveryAddress).HasMaxLength(300);

        b.Property(x => x.SubTotal).HasPrecision(18, 2);
        b.Property(x => x.DiscountAmount).HasPrecision(18, 2);
        b.Property(x => x.TotalAmount).HasPrecision(18, 2);
        b.Property(x => x.CommissionRate).HasPrecision(18, 4);
        b.Property(x => x.CommissionAmount).HasPrecision(18, 2);

        b.HasIndex(x => x.OrderCode).IsUnique();
        b.HasIndex(x => x.Status);

        b.HasOne(x => x.Customer)
            .WithMany(u => u.Orders)
            .HasForeignKey(x => x.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Shop)
            .WithMany(s => s.Orders)
            .HasForeignKey(x => x.ShopId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Quote)
            .WithMany()
            .HasForeignKey(x => x.QuoteId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Machine)
            .WithMany(m => m.Orders)
            .HasForeignKey(x => x.MachineId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.Voucher)
            .WithMany(v => v.Orders)
            .HasForeignKey(x => x.VoucherId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> b)
    {
        b.ToTable("OrderItems");
        b.HasKey(x => x.Id);

        b.Property(x => x.PaperType).HasMaxLength(50);
        b.Property(x => x.BindingType).HasMaxLength(50);
        b.Property(x => x.MaterialName).HasMaxLength(100);
        b.Property(x => x.QualityProfile).HasMaxLength(50);
        b.Property(x => x.ItemNote).HasMaxLength(500);

        b.Property(x => x.UnitPrice).HasPrecision(18, 2);
        b.Property(x => x.LineTotal).HasPrecision(18, 2);
        b.Property(x => x.EstimatedGrams).HasPrecision(18, 3);

        b.HasOne(x => x.Order)
            .WithMany(o => o.Items)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.ServiceType)
            .WithMany()
            .HasForeignKey(x => x.ServiceTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasOne(x => x.DocumentFile)
            .WithMany(d => d.OrderItems)
            .HasForeignKey(x => x.DocumentFileId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class OrderStatusHistoryConfiguration : IEntityTypeConfiguration<OrderStatusHistory>
{
    public void Configure(EntityTypeBuilder<OrderStatusHistory> b)
    {
        b.ToTable("OrderStatusHistories");
        b.HasKey(x => x.Id);

        b.Property(x => x.Reason).HasMaxLength(1000);

        b.HasOne(x => x.Order)
            .WithMany(o => o.StatusHistory)
            .HasForeignKey(x => x.OrderId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.ActorUser)
            .WithMany()
            .HasForeignKey(x => x.ActorUserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
