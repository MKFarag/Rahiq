namespace Infrastructure.Persistence.Configurations;

internal class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        // PrimaryKey

        builder
            .HasKey(c => new { c.ProductId, c.CustomerId });

        // Relationships

        builder
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(c => c.CustomerId);
    }
}
