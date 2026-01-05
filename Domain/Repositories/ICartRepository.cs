namespace Domain.Repositories;

public interface ICartRepository : IGenericRepository<Cart>
{
    Task ExecuteChangeQuantityAsync(int productId, string userId, int quantity);
}
