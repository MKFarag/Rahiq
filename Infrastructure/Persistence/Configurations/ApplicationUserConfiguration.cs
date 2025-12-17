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


        // Properties

        builder
            .Property(u => u.FirstName)
            .HasMaxLength(50);

        builder
            .Property(u => u.LastName)
            .HasMaxLength(50);

        // Default data

        builder
            .HasData(new ApplicationUser
            {
                Id = DefaultUsers.Admin.Id,
                Email = DefaultUsers.Admin.Email,
                NormalizedEmail = DefaultUsers.Admin.Email.ToUpper(),
                EmailConfirmed = true,
                UserName = DefaultUsers.Admin.Email,
                NormalizedUserName = DefaultUsers.Admin.Email.ToUpper(),
                PasswordHash = DefaultUsers.Admin.PasswordHash,
                SecurityStamp = DefaultUsers.Admin.SecurityStamp,
                ConcurrencyStamp = DefaultUsers.Admin.ConcurrencyStamp,
                FirstName = DefaultUsers.Admin.FirstName,
                LastName = DefaultUsers.Admin.LastName,
                IsDisabled = false
            });
    }
}
