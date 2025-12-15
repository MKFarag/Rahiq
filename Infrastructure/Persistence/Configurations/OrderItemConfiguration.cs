namespace Infrastructure.Persistence.Configurations;

internal class OrderItemConfiguration : IEntityTypeConfiguration<OrderItem>
{
    public void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        // Properties

        builder
            .Property(oi => oi.UnitPrice)
            .HasPrecision(5, 2);

        builder
            .Property(oi => oi.Quantity)
            .HasDefaultValue(1);
    }
}
