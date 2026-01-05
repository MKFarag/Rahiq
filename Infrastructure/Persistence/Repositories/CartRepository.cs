namespace Infrastructure.Persistence.Repositories;

public class CartRepository(ApplicationDbContext context) : GenericRepository<Cart>(context), ICartRepository
{
    private readonly ApplicationDbContext _context = context;

    public async Task ExecuteChangeQuantityAsync(int productId, string userId, int quantity)
        => await _context.Carts
            .Where(x => x.ProductId == productId && x.CustomerId == userId)
            .ExecuteUpdateAsync(x => x.SetProperty(c => c.Quantity, quantity));
}
