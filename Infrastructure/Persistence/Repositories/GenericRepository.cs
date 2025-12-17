namespace Infrastructure.Persistence.Repositories;

public class GenericRepository<TEntity>(ApplicationDbContext context) : IGenericRepository<TEntity> where TEntity : class
{
    private readonly DbSet<TEntity> _dbSet = context.Set<TEntity>();
    private readonly ApplicationDbContext _context = context;

    #region Read

    #region Basic

    public async Task<TEntity?> GetAsync(object[] keyValues, CancellationToken cancellationToken = default)
        => await _dbSet.FindAsync(keyValues, cancellationToken);

    public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _dbSet.AsNoTracking().ToListAsync(cancellationToken);

    public async Task<IEnumerable<TEntity>> GetAllAsync(string[] includes, CancellationToken cancellationToken = default)
        => await _dbSet.AsNoTracking().ApplyIncludesSafely(includes).ToListAsync(cancellationToken);

    public async Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => await _dbSet.AsNoTracking().FirstOrDefaultAsync(predicate, cancellationToken);

    public async Task<TEntity?> FindAsync(Expression<Func<TEntity, bool>> predicate, string[] includes, CancellationToken cancellationToken = default)
        => await _dbSet.AsNoTracking().Where(predicate).ApplyIncludesSafely(includes).FirstOrDefaultAsync(cancellationToken);

    public async Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => await _dbSet.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);

    public async Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate, string[] includes, CancellationToken cancellationToken = default)
        => await _dbSet.AsNoTracking().Where(predicate).ApplyIncludesSafely(includes).ToListAsync(cancellationToken);

    public async Task<IEnumerable<TEntity>> FindAllAsync(Expression<Func<TEntity, bool>> predicate, int? take = null, int? skip = null, Expression<Func<TEntity, object>>? orderBy = null, string orderByType = OrderBy.Ascending, CancellationToken cancellationToken = default)
    {
        IQueryable<TEntity> query = _dbSet.AsNoTracking().Where(predicate);

        if (orderBy != null)
            if (string.Equals(orderByType, OrderBy.Ascending, StringComparison.OrdinalIgnoreCase))
                query = query.OrderBy(orderBy);
            else if (string.Equals(orderByType, OrderBy.Descending, StringComparison.OrdinalIgnoreCase))
                query = query.OrderByDescending(orderBy);

        if (skip.HasValue)
            query = query.Skip(skip.Value);

        if (take.HasValue)
            query = query.Take(take.Value);

        return await query.ToListAsync(cancellationToken);
    }

    #endregion

    #region Tracked Find

    public async Task<TEntity?> TrackedFindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);

    public async Task<TEntity?> TrackedFindAsync(Expression<Func<TEntity, bool>> predicate, string[] includes, CancellationToken cancellationToken = default)
        => await _dbSet.Where(predicate).ApplyIncludesSafely(includes).FirstOrDefaultAsync(cancellationToken);

    public async Task<IEnumerable<TEntity>> TrackedFindAllAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => await _dbSet.Where(predicate).ToListAsync(cancellationToken);

    public async Task<IEnumerable<TEntity>> TrackedFindAllAsync(Expression<Func<TEntity, bool>> predicate, string[] includes, CancellationToken cancellationToken = default)
        => await _dbSet.Where(predicate).ApplyIncludesSafely(includes).ToListAsync(cancellationToken);

    #endregion

    #region Projection

    public async Task<IEnumerable<TProjection>> GetAllProjectionAsync<TProjection>(CancellationToken cancellationToken = default) where TProjection : class
        => await _dbSet.AsNoTracking().ProjectToType<TProjection>().ToListAsync(cancellationToken);

    public async Task<IEnumerable<TProjection>> GetAllProjectionAsync<TProjection>(string[] includes, CancellationToken cancellationToken = default) where TProjection : class
        => await _dbSet.AsNoTracking().ApplyIncludesSafely(includes).ProjectToType<TProjection>().ToListAsync(cancellationToken);

    public async Task<IEnumerable<TProjection>> GetAllProjectionAsync<TProjection>(Expression<Func<TEntity, TProjection>> selector, bool distinct, CancellationToken cancellationToken = default)
        => distinct
            ? await _dbSet.AsNoTracking().Select(selector).Distinct().ToListAsync(cancellationToken)
            : await _dbSet.AsNoTracking().Select(selector).ToListAsync(cancellationToken);

    public async Task<IEnumerable<TProjection>> GetAllProjectionAsync<TProjection>(string[] includes, Expression<Func<TEntity, TProjection>> selector, bool distinct, CancellationToken cancellationToken = default)
        => distinct
            ? await _dbSet.AsNoTracking().ApplyIncludesSafely(includes).Select(selector).Distinct().ToListAsync(cancellationToken)
            : await _dbSet.AsNoTracking().ApplyIncludesSafely(includes).Select(selector).ToListAsync(cancellationToken);

    public async Task<IEnumerable<TProjection>> FindAllProjectionAsync<TProjection>(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default) where TProjection : class
        => await _dbSet.AsNoTracking().Where(predicate).ProjectToType<TProjection>().ToListAsync(cancellationToken);

    public async Task<IEnumerable<TProjection>> FindAllProjectionAsync<TProjection>(Expression<Func<TEntity, bool>> predicate, string[] includes, CancellationToken cancellationToken = default) where TProjection : class
        => await _dbSet.AsNoTracking().Where(predicate).ApplyIncludesSafely(includes).ProjectToType<TProjection>().ToListAsync(cancellationToken);

    public async Task<IEnumerable<TProjection>> FindAllProjectionAsync<TProjection>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TProjection>> selector, bool distinct, CancellationToken cancellationToken = default)
        => distinct
            ? await _dbSet.AsNoTracking().Where(predicate).Select(selector).Distinct().ToListAsync(cancellationToken)
            : await _dbSet.AsNoTracking().Where(predicate).Select(selector).ToListAsync(cancellationToken);

    public async Task<IEnumerable<TProjection>> FindAllProjectionAsync<TProjection>(Expression<Func<TEntity, bool>> predicate, string[] includes, Expression<Func<TEntity, TProjection>> selector, bool distinct, CancellationToken cancellationToken = default)
        => distinct
            ? await _dbSet.AsNoTracking().Where(predicate).ApplyIncludesSafely(includes).Select(selector).Distinct().ToListAsync(cancellationToken)
            : await _dbSet.AsNoTracking().Where(predicate).ApplyIncludesSafely(includes).Select(selector).ToListAsync(cancellationToken);

    #endregion

    #endregion

    #region Modify

    public async Task<TEntity> AddAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        return entity;
    }

    public async Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddRangeAsync(entities, cancellationToken);
        return entities;
    }

    public void Delete(TEntity entity)
        => _dbSet.Remove(entity);

    public void DeleteRange(IEnumerable<TEntity> entities)
        => _dbSet.RemoveRange(entities);

    public async Task ExecuteDeleteAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => await _dbSet.Where(predicate).ExecuteDeleteAsync(cancellationToken);

    public TEntity Update(TEntity entity)
    {
        _dbSet.Update(entity);
        return entity;
    }

    public IEnumerable<TEntity> UpdateRange(IEnumerable<TEntity> entities)
    {
        _dbSet.UpdateRange(entities);
        return entities;
    }

    #endregion

    #region Helpers

    public async Task<int> CountAsync(CancellationToken cancellationToken = default)
        => await _dbSet.CountAsync(cancellationToken);

    public async Task<int> CountAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => await _dbSet.CountAsync(predicate, cancellationToken);

    public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        => await _dbSet.AnyAsync(predicate, cancellationToken);

    public void Attach(TEntity entity)
        => _dbSet.Attach(entity);

    public void AttachRange(IEnumerable<TEntity> entities)
        => _dbSet.AttachRange(entities);

    public void MarkPropertyAsModified<TProperty>(TEntity entity, Expression<Func<TEntity, TProperty>> property)
    {
        var entry = _context.Entry(entity);

        if (entry.State == EntityState.Detached)
            _dbSet.Attach(entity);

        entry.Property(property).IsModified = true;
    }

    #endregion
}