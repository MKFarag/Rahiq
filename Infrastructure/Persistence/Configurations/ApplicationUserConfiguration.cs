using Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations;

internal class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        // Relationship with RefreshToken

        builder
            .OwnsMany(u => u.RefreshTokens)
            .ToTable(nameof(RefreshToken) + "s")
            .WithOwner()
            .HasForeignKey("UserId");

        //////////////////////////////////////////////////////////////////////////

        // Properties

        builder
            .Property(u => u.FirstName)
            .HasMaxLength(50);

        builder
            .Property(u => u.LastName)
            .HasMaxLength(50);

        //////////////////////////////////////////////////////////////////////////
    }
}
