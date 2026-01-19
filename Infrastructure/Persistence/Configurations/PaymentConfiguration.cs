namespace Infrastructure.Persistence.Configurations;

internal class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        // Properties

        builder
            .Property(p => p.Amount)
            .HasPrecision(10, 2);
    }
}
