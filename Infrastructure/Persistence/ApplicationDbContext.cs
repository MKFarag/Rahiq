using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.Reflection;

namespace Infrastructure.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext<ApplicationUser, ApplicationRole, string>(options)
{
    #region DbSet

    public DbSet<BundleItem> BundleItems { get; set; }
    public DbSet<Bundle> Bundles { get; set; }
    public DbSet<Cart> Carts { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<Product> Products { get; set; }
    public DbSet<Shipping> Shipping { get; set; }
    public DbSet<Domain.Entities.Type> Types { get; set; }

    #endregion

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

        #region Change delete behavior

        var cascadeFk = builder.Model
            .GetEntityTypes()
            .SelectMany(entityType => entityType.GetForeignKeys())
            .Where(Fk => Fk.DeleteBehavior == DeleteBehavior.Cascade && !Fk.IsOwnership);

        foreach (var fk in cascadeFk)
            fk.DeleteBehavior = DeleteBehavior.Restrict;

        #endregion

        base.OnModelCreating(builder);
    }
}
