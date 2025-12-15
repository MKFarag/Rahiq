namespace Infrastructure.Persistence.Configurations;

internal class BundleItemConfiguration : IEntityTypeConfiguration<BundleItem>
{
    public void Configure(EntityTypeBuilder<BundleItem> builder)
    {
        // PrimaryKey

        builder.HasKey(bi => new { bi.BundleId, bi.ProductId });
    }
}
