using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrintHub.Domain.Entities;

namespace PrintHub.Infrastructure.Persistence.Configurations;

public class ShopGalleryImageConfiguration : IEntityTypeConfiguration<ShopGalleryImage>
{
    public void Configure(EntityTypeBuilder<ShopGalleryImage> b)
    {
        b.ToTable("ShopGalleryImages");
        b.HasKey(x => x.Id);
        b.Property(x => x.Url).HasMaxLength(500).IsRequired();
        b.Property(x => x.Caption).HasMaxLength(200);
        b.HasOne(x => x.Shop).WithMany(s => s.GalleryImages).HasForeignKey(x => x.ShopId).OnDelete(DeleteBehavior.Cascade);
    }
}
