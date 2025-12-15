namespace Infrastructure.Persistence.Configurations;

internal class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        // Properties

        builder
            .Property(p => p.Name)
            .HasMaxLength(150);

        builder
            .Property(p => p.Brand)
            .HasMaxLength(50);

        builder
            .Property(p => p.Price)
            .HasPrecision(5, 2);

        // Index

        builder
            .HasIndex(p => p.Name)
            .IsUnique();
    }
}
