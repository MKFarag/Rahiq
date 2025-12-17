namespace Domain.Repositories;

/// <summary>Generic repository interface for CRUD operations</summary>
/// <typeparam name="TEntity">The entity type</typeparam>
public interface IGenericRepository<TEntity> where TEntity : class
{
    #region Read

    #region Basic

    /// <summary>Get trackable entity by ID</summary>
    /// <param name="keyValues">Entity IDs</param>
    Task<TEntity?> GetAsync(object[] keyValues, CancellationToken cancellationToken = default);

    /// <summary>Get all entities</summary>
    Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>Get all entities</summary>
    /// <param name="includes">Related data to include</param>
    Task<IEnumerable<TEntity>> GetAllAsync(string[] includes, CancellationToken cancellationToken = default);

    /// <summary>Find first entity matching condition</summary>
    /// <param name="predicate">Filter condition</param>
    Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>Find first entity matching condition</summary>
    /// <param name="predicate">Filter condition</param>
    /// <param name="includes">Related data to include</param>
    Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate, string[] includes, CancellationToken cancellationToken = default);

    /// <summary>Find all entities matching condition</summary>
    /// <param name="predicate">Filter condition</param>
    Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>Find all entities matching condition</summary>
    /// <param name="predicate">Filter condition</param>
    /// <param name="includes">Related data to include</param>
    Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate, string[] includes, CancellationToken cancellationToken = default);

    /// <summary>Find entities with ordering and pagination</summary>
    /// <param name="predicate">Filter condition</param>
    /// <param name="take">Number to take</param>
    /// <param name="skip">Number to skip</param>
    /// <param name="orderBy">Order by expression</param>
    /// <param name="orderByType">Sort direction</param>
    Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate, int? take = null, int? skip = null, Expression<Func<TEntity, object>>? orderBy = null, string orderByType = OrderBy.Ascending, CancellationToken cancellationToken = default);

    #endregion

    #region Tracked Find

    /// <summary>Find first trackable entity matching condition</summary>
    /// <param name="predicate">Filter condition</param>
    Task<TEntity?> TrackedFindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>Find first trackable entity matching condition</summary>
    /// <param name="predicate">Filter condition</param>
    /// <param name="includes">Related data to include</param>
    Task<TEntity?> TrackedFindAsync(Expression<Func<TEntity, bool>> predicate, string[] includes, CancellationToken cancellationToken = default);

    /// <summary>Find all trackable entities matching condition</summary>
    /// <param name="predicate">Filter condition</param>
    Task<IEnumerable<TEntity>> TrackedFindAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>Find all trackable entities matching condition</summary>
    /// <param name="predicate">Filter condition</param>
    /// <param name="includes">Related data to include</param>
    Task<IEnumerable<TEntity>> TrackedFindAllAsync(Expression<Func<TEntity, bool>> predicate, string[] includes, CancellationToken cancellationToken = default);

    #endregion

    #region Projection

    /// <summary>Get all entities converted to DTO</summary>
    /// <typeparam name="TProjection">The type of the projection</typeparam>
    Task<IEnumerable<TProjection>> GetAllProjectionAsync<TProjection>(CancellationToken cancellationToken = default) where TProjection : class;

    /// <summary>Get all entities converted to DTO</summary>
    /// <typeparam name="TProjection">The type of the projection</typeparam>
    /// <param name="includes">Related data to include</param>
    Task<IEnumerable<TProjection>> GetAllProjectionAsync<TProjection>(string[] includes, CancellationToken cancellationToken = default) where TProjection : class;

    /// <summary>Get all entities with custom projection</summary>
    /// <param name="selector">Custom projection expression</param>
    /// <param name="distinct">Apply distinct to results</param>
    Task<IEnumerable<TProjection>> GetAllProjectionAsync<TProjection>(Expression<Func<TEntity, TProjection>> selector, bool distinct, CancellationToken cancellationToken = default);

    /// <summary>Get all entities with custom projection</summary>
    /// <param name="selector">Custom projection expression</param>
    /// <param name="distinct">Apply distinct to results</param>
    /// <param name="includes">Related data to include</param>
    Task<IEnumerable<TProjection>> GetAllProjectionAsync<TProjection>(string[] includes, Expression<Func<TEntity, TProjection>> selector, bool distinct, CancellationToken cancellationToken = default);

    /// <summary>Find all entities with your DTO</summary>
    /// <param name="predicate">Filter condition</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task<IEnumerable<TProjection>> FindAllProjectionAsync<TProjection>(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) where TProjection : class;

    /// <summary>Find all entities converted to DTO</summary>
    /// <param name="predicate">Filter condition</param>
    /// <param name="includes">Related data to include</param
    Task<IEnumerable<TProjection>> FindAllProjectionAsync<TProjection>(Expression<Func<TEntity, bool>> predicate, string[] includes, CancellationToken cancellationToken = default) where TProjection : class;

    /// <summary>Find all entities with custom projection</summary>
    /// <param name="predicate">Filter condition</param>
    /// <param name="selector">Custom projection expression</param>
    /// <param name="distinct">Apply distinct to results</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task<IEnumerable<TProjection>> FindAllProjectionAsync<TProjection>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TProjection>> selector, bool distinct, CancellationToken cancellationToken = default);

    /// <summary>Find all entities with custom projection</summary>
    /// <param name="predicate">Filter condition</param>
    /// <param name="includes">Related data to include</param>
    /// <param name="selector">Custom projection expression</param>
    /// <param name="distinct">Apply distinct to results</param>
    Task<IEnumerable<TProjection>> FindAllProjectionAsync<TProjection>(Expression<Func<TEntity, bool>> predicate, string[] includes, Expression<Func<TEntity, TProjection>> selector, bool distinct, CancellationToken cancellationToken = default);

    #endregion

    #endregion

    #region Modify

    /// <summary>Create new entity</summary>
    /// <param name="entity">Entity to add</param>
    Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default);

    /// <summary>Create multiple entities</summary>
    /// <param name="entities">Entities to add</param>
    Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default);

    /// <summary>Delete entity</summary>
    /// <param name="entity">Entity to delete</param>
    void Delete(TEntity entity);

    /// <summary>Delete multiple entities</summary>
    /// <param name="entities">Entities to delete</param>
    void DeleteRange(IEnumerable<TEntity> entities);

    /// <summary>Execute delete command directly in the database for entities matching the predicate</summary>
    /// <param name="predicate">Filter condition</param>
    Task ExecuteDeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>Update entity</summary>
    /// <param name="entity">Entity to update</param>
    TEntity Update(TEntity entity);

    /// <summary>Update multiple entities</summary>
    /// <param name="entities">Entities to update</param>
    IEnumerable<TEntity> UpdateRange(IEnumerable<TEntity> entities);

    #endregion

    #region Helpers

    /// <summary>Count all entities</summary>
    Task<int> CountAsync(CancellationToken cancellationToken = default);

    /// <summary>Count entities matching condition</summary>
    /// <param name="predicate">Filter condition</param>
    Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>Check if any entities match condition</summary>
    /// <param name="predicate">Filter condition</param>
    Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default);

    /// <summary>Attach entity to context for tracking</summary>
    /// <param name="entity">Entity to attach</param>
    void Attach(TEntity entity);

    /// <summary>Attach multiple entities to context for tracking</summary>
    /// <param name="entities">Entities to attach</param>
    void AttachRange(IEnumerable<TEntity> entities);

    /// <summary>Mark a property as modified</summary>
    /// <param name="entity">Entity to mark property as modified</param>
    /// <param name="property">Property to mark as modified</param>
    void MarkPropertyAsModified<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> property);

    #endregion
}
