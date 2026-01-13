namespace Infrastructure.Persistence.Configurations;

internal class CartConfiguration : IEntityTypeConfiguration<Cart>
{
    public void Configure(EntityTypeBuilder<Cart> builder)
    {
        builder.HasKey(c => c.Id);

        builder
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(c => c.CustomerId);

        builder.HasIndex(c => new { c.CustomerId, c.ProductId })
            .IsUnique()
            .HasFilter("[ProductId] IS NOT NULL");

        builder.HasIndex(c => new { c.CustomerId, c.BundleId })
            .IsUnique()
            .HasFilter("[BundleId] IS NOT NULL");
    }
}
