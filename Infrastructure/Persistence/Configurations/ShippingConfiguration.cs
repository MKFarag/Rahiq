namespace Infrastructure.Persistence.Configurations;

internal class ShippingConfiguration : IEntityTypeConfiguration<Shipping>
{
    public void Configure(EntityTypeBuilder<Shipping> builder)
    {
        // Properties

        builder
            .Property(s => s.Phone)
            .HasMaxLength(11);

        builder
            .Property(s => s.Cost)
            .HasPrecision(4, 2);

        builder
            .Property(s => s.Code)
            .HasMaxLength(50);

        //  Indexes

        builder
            .HasIndex(s => s.Code)
            .IsUnique();
    }
}
