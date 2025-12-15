namespace Infrastructure.Persistence.Configurations;

internal class TypeConfiguration : IEntityTypeConfiguration<Domain.Entities.Type>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Type> builder)
    {
        // Properties

        builder
            .Property(t => t.Name)
            .HasMaxLength(50);

        //  Indexes

        builder
            .HasIndex(t => t.Name)
            .IsUnique();
    }
}
