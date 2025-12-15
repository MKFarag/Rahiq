namespace Infrastructure.Persistence.Configurations;

internal class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        // Properties

        builder
            .Property(c => c.Name)
            .HasMaxLength(50);

        //  Indexes

        builder
            .HasIndex(c => c.Name)
            .IsUnique();
    }
}
