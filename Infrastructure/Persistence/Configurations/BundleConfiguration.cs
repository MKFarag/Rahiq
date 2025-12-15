namespace Infrastructure.Persistence.Configurations;

internal class BundleConfiguration : IEntityTypeConfiguration<Bundle>
{
    public void Configure(EntityTypeBuilder<Bundle> builder)
    {
        // Properties

        builder
            .Property(b => b.Name)
            .HasMaxLength(200);
    }
}
