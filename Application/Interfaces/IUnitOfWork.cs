namespace Application.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<Category> Categories { get; }
    IRoleRepository Roles { get; }
    IUserRepository Users { get; }

    /// <summary>Save changes to the database</summary>
    /// <returns>The number of state entries written to the database</returns>
    int Complete();

    /// <summary>Save changes to the database</summary>
    /// <returns>The number of state entries written to the database</returns>
    Task<int> CompleteAsync(CancellationToken cancellationToken = default);
}
