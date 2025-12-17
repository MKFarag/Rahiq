namespace Infrastructure.Persistence.Configurations;

internal class ApplicationRoleConfiguration : IEntityTypeConfiguration<ApplicationRole>
{
    public void Configure(EntityTypeBuilder<ApplicationRole> builder)
    {
        // Default data

        builder.HasData(new ApplicationRole
        {
            Id = DefaultRoles.Admin.Id,
            Name = DefaultRoles.Admin.Name,
            NormalizedName = DefaultRoles.Admin.Name.ToUpper(),
            ConcurrencyStamp = DefaultRoles.Admin.ConcurrencyStamp,
            IsDefault = false,
        },
        new ApplicationRole
        {
            Id = DefaultRoles.Customer.Id,
            Name = DefaultRoles.Customer.Name,
            NormalizedName = DefaultRoles.Customer.Name.ToUpper(),
            ConcurrencyStamp = DefaultRoles.Customer.ConcurrencyStamp,
            IsDefault = true,
        });
    }
}
