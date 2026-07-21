using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrintHub.Domain.Entities;

namespace PrintHub.Infrastructure.Persistence.Configurations;

public class ServiceTypeConfiguration : IEntityTypeConfiguration<ServiceType>
{
    public void Configure(EntityTypeBuilder<ServiceType> b)
    {
        b.ToTable("ServiceTypes");
        b.HasKey(x => x.Id);

        b.Property(x => x.Code).HasMaxLength(50).IsRequired();
        b.Property(x => x.Name).HasMaxLength(150).IsRequired();
        b.Property(x => x.UnitOfMeasure).HasMaxLength(20).IsRequired();
        b.Property(x => x.Description).HasMaxLength(1000);
        b.Property(x => x.IconUrl).HasMaxLength(500);

        b.HasIndex(x => x.Code).IsUnique();

        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

/// <summary>
/// The rate-card junction (Shop x ServiceType with attributes) — the central
/// many-to-many-with-attributes relationship, read on every quote computation.
/// </summary>
public class ShopServiceConfiguration : IEntityTypeConfiguration<ShopService>
{
    public void Configure(EntityTypeBuilder<ShopService> b)
    {
        b.ToTable("ShopServices");
        b.HasKey(x => x.Id);

        b.Property(x => x.UnitPrice).HasPrecision(18, 2);
        b.Property(x => x.SetupFee).HasPrecision(18, 2);
        b.Property(x => x.Notes).HasMaxLength(500);

        // A shop offers a given service type at most once.
        b.HasIndex(x => new { x.ShopId, x.ServiceTypeId }).IsUnique();

        b.HasOne(x => x.Shop)
            .WithMany(s => s.Services)
            .HasForeignKey(x => x.ShopId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.ServiceType)
            .WithMany(t => t.ShopServices)
            .HasForeignKey(x => x.ServiceTypeId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}

public class PriceRuleConfiguration : IEntityTypeConfiguration<PriceRule>
{
    public void Configure(EntityTypeBuilder<PriceRule> b)
    {
        b.ToTable("PriceRules");
        b.HasKey(x => x.Id);

        b.Property(x => x.OptionKey).HasMaxLength(50).IsRequired();
        b.Property(x => x.Multiplier).HasPrecision(18, 4);
        b.Property(x => x.FlatExtra).HasPrecision(18, 2);
        b.Property(x => x.Description).HasMaxLength(200);

        b.HasOne(x => x.ShopService)
            .WithMany(s => s.PriceRules)
            .HasForeignKey(x => x.ShopServiceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
