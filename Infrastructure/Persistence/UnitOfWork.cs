using Infrastructure.Persistence.Repositories;

namespace Infrastructure.Persistence;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private bool _disposed = false;

    public IGenericRepository<Bundle> Bundles { get; private set; }
    public IGenericRepository<BundleItem> BundleItems { get; private set; }
    public IGenericRepository<Cart> Carts { get; private set; }
    public IGenericRepository<Category> Categories { get; private set; }
    public IGenericRepositoryWithPagination<Order> Orders { get; private set; }
    public IGenericRepository<OrderItem> OrderItems { get; private set; }
    public IGenericRepositoryWithPagination<Product> Products { get; private set; }
    public IRoleRepository Roles { get; private set; }
    public IGenericRepository<Shipping> Shipping { get; private set; }
    public IGenericRepository<Domain.Entities.Type> Types { get; private set; }
    public IUserRepository Users { get; private set; }

    public UnitOfWork(
        ApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager)
    {
        _context = context;

        Bundles = new GenericRepository<Bundle>(_context);
        BundleItems = new GenericRepository<BundleItem>(_context);
        Carts = new GenericRepository<Cart>(_context);
        Categories = new GenericRepository<Category>(_context);
        Orders = new GenericRepositoryWithPagination<Order>(_context);
        OrderItems = new GenericRepository<OrderItem>(_context);
        Products = new GenericRepositoryWithPagination<Product>(_context);
        Roles = new RoleRepository(_context, roleManager);
        Shipping = new GenericRepository<Shipping>(_context);
        Types = new GenericRepository<Domain.Entities.Type>(_context);
        Users = new UserRepository(_context, userManager);
    }

    public int Complete()
        => _context.SaveChanges();

    public async Task<int> CompleteAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken);

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
                _context?.Dispose();

            _disposed = true;
        }
    }
}
