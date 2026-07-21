using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using PrintHub.Domain.Entities;

namespace PrintHub.Infrastructure.Persistence.Configurations;

public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> b)
    {
        b.ToTable("Users");
        b.HasKey(x => x.Id);

        b.Property(x => x.FullName).HasMaxLength(100).IsRequired();
        b.Property(x => x.Email).HasMaxLength(256).IsRequired();
        b.Property(x => x.PasswordHash).HasMaxLength(200).IsRequired();
        b.Property(x => x.PhoneNumber).HasMaxLength(20);
        b.Property(x => x.DefaultAddress).HasMaxLength(300);
        b.Property(x => x.WalletBalance).HasPrecision(18, 2);
        b.Property(x => x.AvatarUrl).HasMaxLength(500);

        b.HasIndex(x => x.Email).IsUnique();

        b.HasQueryFilter(x => !x.IsDeleted);
    }
}

public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> b)
    {
        b.ToTable("RefreshTokens");
        b.HasKey(x => x.Id);

        b.Property(x => x.Token).HasMaxLength(500).IsRequired();
        b.Property(x => x.CreatedByIp).HasMaxLength(45);
        b.Property(x => x.RevokedByIp).HasMaxLength(45);
        b.Property(x => x.ReplacedByToken).HasMaxLength(500);
        b.HasIndex(x => x.Token).IsUnique();

        b.Ignore(x => x.IsActive);

        b.HasOne(x => x.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(x => x.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
