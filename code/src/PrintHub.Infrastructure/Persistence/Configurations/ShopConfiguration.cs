using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrintHub.Domain.Entities;

namespace PrintHub.Infrastructure.Persistence.Configurations;

public class ShopConfiguration : IEntityTypeConfiguration<Shop>
{
    public void Configure(EntityTypeBuilder<Shop> b)
    {
        b.ToTable("Shops");
        b.HasKey(x => x.Id);

        b.Property(x => x.Name).HasMaxLength(200).IsRequired();
        b.Property(x => x.Description).HasMaxLength(2000);
        b.Property(x => x.AddressLine).HasMaxLength(300).IsRequired();
        b.Property(x => x.District).HasMaxLength(100).IsRequired();
        b.Property(x => x.City).HasMaxLength(100).IsRequired();
        b.Property(x => x.PhoneNumber).HasMaxLength(20);
        b.Property(x => x.ReviewNote).HasMaxLength(1000);

        b.HasIndex(x => x.Status);
        b.HasIndex(x => x.District);

        b.HasOne(x => x.Owner)
            .WithMany(u => u.OwnedShops)
            .HasForeignKey(x => x.OwnerId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class ShopStaffConfiguration : IEntityTypeConfiguration<ShopStaff>
{
    public void Configure(EntityTypeBuilder<ShopStaff> b)
    {
        b.ToTable("ShopStaff");
        b.HasKey(x => x.Id);

        b.Property(x => x.Position).HasMaxLength(100);

        // A user is staff of a given shop at most once.
        b.HasIndex(x => new { x.ShopId, x.UserId }).IsUnique();

        b.HasOne(x => x.Shop)
            .WithMany(s => s.Staff)
            .HasForeignKey(x => x.ShopId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.User)
            .WithMany(u => u.StaffMemberships)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class MachineConfiguration : IEntityTypeConfiguration<Machine>
{
    public void Configure(EntityTypeBuilder<Machine> b)
    {
        b.ToTable("Machines");
        b.HasKey(x => x.Id);

        b.Property(x => x.Name).HasMaxLength(100).IsRequired();

        b.HasOne(x => x.Shop)
            .WithMany(s => s.Machines)
            .HasForeignKey(x => x.ShopId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class MaterialConfiguration : IEntityTypeConfiguration<Material>
{
    public void Configure(EntityTypeBuilder<Material> b)
    {
        b.ToTable("Materials");
        b.HasKey(x => x.Id);

        b.Property(x => x.Name).HasMaxLength(100).IsRequired();
        b.Property(x => x.Unit).HasMaxLength(20).IsRequired();
        b.Property(x => x.StockQuantity).HasPrecision(18, 3);
        b.Property(x => x.LowStockThreshold).HasPrecision(18, 3);
        b.Property(x => x.UnitCost).HasPrecision(18, 2);

        b.Ignore(x => x.IsLowStock);

        b.HasOne(x => x.Shop)
            .WithMany(s => s.Materials)
            .HasForeignKey(x => x.ShopId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasQueryFilter(x => !x.IsDeleted);
    }
}
