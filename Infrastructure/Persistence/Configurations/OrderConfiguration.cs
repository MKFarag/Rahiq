namespace Infrastructure.Persistence.Configurations;

internal class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        // Relationships

        builder
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(o => o.CustomerId);

        // Properties

        builder
            .Property(o => o.Status)
            .HasConversion<string>()
            .HasMaxLength(50);

        builder
            .Property(o => o.Total)
            .HasPrecision(10, 2);

    }
}
