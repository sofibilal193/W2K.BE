using System.Linq.Expressions;
using W2K.Common.Entities;
using W2K.Common.Exceptions;
using W2K.Common.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;

namespace W2K.Common.Persistence.Repositories;

/// <summary>
/// Base repository for managing entity models configured with DbContext.
/// </summary>
/// <typeparam name="TEntity">Entity model type</typeparam>
/// <remarks>
/// Creates a new database repository with the specified context.
/// </remarks>
/// <param name="context">Database context containing entity definitions.</param>
public abstract class DbRepository<TEntity>(DbContext context) : IDbRepository<TEntity> where TEntity : BaseEntity
{
    private readonly DbContext _context = context;
    private readonly List<Expression<Func<TEntity, object>>> _includes = [];
    private readonly List<string> _stringIncludes = [];
    private bool _trackChanges = true;

    protected IQueryable<TEntity> Entities
    {
        get
        {
            var entities = _context.Set<TEntity>().AsQueryable();
            if (!_trackChanges)
            {
                entities = entities.AsNoTracking();
                _trackChanges = true;
            }
            if (_includes.Count > 0)
            {
                entities = _includes.Aggregate(entities, (current, include) => current.Include(include));
                _includes.Clear();
            }
            if (_stringIncludes.Count > 0)
            {
                entities = _stringIncludes.Aggregate(entities, (current, include) => current.Include(include));
                _stringIncludes.Clear();
            }
            return entities;
        }
    }

    #region Specifications

    /// <summary>
    /// The change tracker will not track any of the entities that are returned from
    /// a LINQ query. If the entity instances are modified, this will not be detected
    /// by the change tracker and Microsoft.EntityFrameworkCore.DbContext.SaveChanges
    /// will not persist those changes to the database.
    /// </summary>
    /// <returns>The same IDbRepository implementation so that multiple calls can be chained.</returns>
    public IDbRepository<TEntity> AsNoTracking()
    {
        _trackChanges = false;
        return this;
    }

    /// <summary>
    /// Specifies related entities to include in the query results. The navigation property
    /// to be included is specified starting with the type of entity being queried (TEntity).
    /// </summary>
    /// <param name="navigationPropertyPath">A lambda expression representing the navigation
    /// property to be included (t => t.Property1).</param>
    /// <returns>The same IDbRepository implementation so that multiple calls can be chained.</returns>
    public IDbRepository<TEntity> Include(Expression<Func<TEntity, object?>> navigationPropertyPath)
    {
        _includes.Add(navigationPropertyPath!);
        return this;
    }

    /// <summary>
    /// Specifies related entities to include in the query results. The navigation property
    /// to be included is specified starting with the type of entity being queried (TEntity).
    /// Further navigation properties to be included can be appended, separated by the '.' character.
    /// </summary>
    /// <param name="navigationPropertyPath">A string of '.' separated navigation property names to be included.</param>
    /// <returns>The same IDbRepository implementation so that multiple calls can be chained.</returns>
    public IDbRepository<TEntity> Include(string navigationPropertyPath)
    {
        _stringIncludes.Add(navigationPropertyPath);
        return this;
    }

    #endregion

    #region Find

    /// <summary>
    /// Finds an entity with the given primary key values. If an entity with the given
    /// primary key values is being tracked by the context, then it is returned immediately
    /// without making a request to the database. Otherwise, a query is made to the database
    /// for an entity with the given primary key values and this entity, if found, is
    /// attached to the context and returned. If no entity is found, then null is returned.
    /// </summary>
    /// <param name="keyValues">The values of the primary key for the entity to be found.</param>
    /// <returns>The entity found, or null.</returns>
    public virtual async Task<TEntity?> FindAsync(params object?[]? keyValues)
    {
        if (!_trackChanges)
        {
            throw new DomainException("FindAsync method cannot be used with AsNoTracking method.");
        }
        if (_includes.Count > 0 || _stringIncludes.Count > 0)
        {
            throw new DomainException("FindAsync method cannot be used with Include method.");
        }
        return await _context.Set<TEntity>().FindAsync(keyValues);
    }

    #endregion

    #region Get

    /// <summary>
    /// Gets all entities filtered by a predicate.
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="cancel">A System.Threading.CancellationToken to observe while waiting for the task to complete.</param>
    /// <returns>List of entities.</returns>
    public virtual async Task<List<TEntity>> GetAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancel = default)
    {
        return await Entities.Where(predicate).ToListAsync(cancel);
    }

    /// <summary>
    /// Gets all entities filtered by a predicate and sorted.
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="sortKeySelector">Function to extract key to sort by.</param>
    /// <param name="sortDescending">Whether to sort in descending order or not.</param>
    /// <param name="cancel">A System.Threading.CancellationToken to observe while waiting for the task to complete.</param>
    /// <returns>List of entities.</returns>
    public virtual async Task<List<TEntity>> GetAsync<TKey>(
         Expression<Func<TEntity, bool>> predicate,
         Expression<Func<TEntity, TKey>> sortKeySelector,
         bool sortDescending = false,
         CancellationToken cancel = default)
    {
        return await Entities.Where(predicate).OrderBy(sortKeySelector, sortDescending).ToListAsync(cancel);
    }

    /// <summary>
    /// Gets an entity with the specified Id.
    /// </summary>
    /// <param name="id">Id of entity to get.</param>
    /// <param name="cancel">A System.Threading.CancellationToken to observe while waiting for the task to complete.</param>
    /// <returns>The entity found, or null.</returns>
    public async Task<TEntity?> GetAsync(int id, CancellationToken cancel = default)
    {
        return await Entities.FirstOrDefaultAsync(x => x.Id == id, cancel);
    }

    #endregion

    #region GetAll

    /// <summary>
    /// Gets all entities.
    /// </summary>
    /// <param name="cancel">A System.Threading.CancellationToken to observe while waiting for the task to complete.</param>
    /// <returns>List of entities.</returns>
    public virtual async Task<List<TEntity>> GetAllAsync(CancellationToken cancel = default)
    {
        return await Entities.ToListAsync(cancel);
    }

    /// <summary>
    /// Gets all entities sorted by a key.
    /// </summary>
    /// <param name="sortKeySelector">Function to extract key to sort by.</param>
    /// <param name="sortDescending">Whether to sort in descending order or not.</param>
    /// <param name="cancel">A System.Threading.CancellationToken to observe while waiting for the task to complete.</param>
    /// <returns>Sorted list of entities.</returns>
    public virtual async Task<List<TEntity>> GetAllAsync<TKey>(
         Expression<Func<TEntity, TKey>> sortKeySelector,
         bool sortDescending = false,
         CancellationToken cancel = default)
    {
        return await Entities.OrderBy(sortKeySelector, sortDescending).ToListAsync(cancel);
    }

    #endregion

    #region GetPaged

    /// <summary>
    /// Gets a collection entities with the requested sorting, filtering, and pagination
    /// </summary>
    /// <param name="page">Number of the page to return</param>
    /// <param name="pageSize">Number of items returned per page.</param>
    /// <param name="sortKeySelector">Function to extract key to sort by.</param>
    /// <param name="sortDescending">Whether to sort in descending order or not.</param>
    /// <param name="cancel">A System.Threading.CancellationToken to observe while waiting for the task to complete.</param>
    /// <returns>Paged list of entities.</returns>
    public virtual async Task<PagedList<TEntity>> GetPagedAsync<TKey>(
         int page,
         int pageSize,
         Expression<Func<TEntity, TKey>> sortKeySelector,
         bool sortDescending = false,
         CancellationToken cancel = default)
    {
        return await Entities.OrderBy(sortKeySelector, sortDescending).ToPagedListAsync(page, pageSize, cancel);
    }

    /// <summary>
    /// Gets all entities filtered by a predicate.
    /// </summary>
    /// <param name="page">Number of the page to return</param>
    /// <param name="pageSize">Number of items returned per page.</param>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="sortKeySelector">Function to extract key to sort by.</param>
    /// <param name="sortDescending">Whether to sort in descending order or not.</param>
    /// <param name="cancel">A System.Threading.CancellationToken to observe while waiting for the task to complete.</param>
    /// <returns>Paged, filtered, and sorted list of entities.</returns>
    public virtual async Task<PagedList<TEntity>> GetPagedAsync<TKey>(
         int page,
         int pageSize,
         Expression<Func<TEntity, bool>> predicate,
         Expression<Func<TEntity, TKey>> sortKeySelector,
         bool sortDescending = false,
         CancellationToken cancel = default)
    {
        return await Entities.Where(predicate).OrderBy(sortKeySelector, sortDescending).ToPagedListAsync(page, pageSize, cancel);
    }

    #endregion

    #region Other

    /// <summary>
    /// Gets a single entity filtered by a predicate or null of none are found.
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="cancel">A System.Threading.CancellationToken to observe while waiting for the task to complete.</param>
    /// <returns>Single Entity.</returns>
    public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancel = default)
    {
        return await Entities.FirstOrDefaultAsync(predicate, cancel);
    }

    /// <summary>
    /// Gets a single entity filtered by a predicate or null of none are found.
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="sortKeySelector">Function to extract key to sort by.</param>
    /// <param name="sortDescending">Whether to sort in descending order or not.</param>
    /// <param name="cancel">A System.Threading.CancellationToken to observe while waiting for the task to complete.</param>
    /// <returns>Single Entity.</returns>
    public async Task<TEntity?> FirstOrDefaultAsync<TKey>(
         Expression<Func<TEntity, bool>> predicate,
         Expression<Func<TEntity, TKey>> sortKeySelector,
         bool sortDescending = false,
         CancellationToken cancel = default)
    {
        return await Entities.Where(predicate).OrderBy(sortKeySelector, sortDescending).FirstOrDefaultAsync(cancel);
    }

    /// <summary>
    /// Asynchronously determines whether any entity satisfies a condition.
    /// </summary>
    /// <param name="predicate">A function to test each element for a condition.</param>
    /// <param name="cancel">A System.Threading.CancellationToken to observe while waiting for the task to complete.</param>
    /// <returns>A task that represents the asynchronous operation.
    /// The task result contains true if any elements in the source sequence pass
    /// the test in the specified predicate; otherwise, false.</returns>
    public virtual async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancel = default)
    {
        return await Entities.AnyAsync(predicate, cancel);
    }

    /// <summary>
    /// Gets all tracked entities that have been added or modified
    /// (Must be called before SaveChanges).
    /// </summary>
    /// <returns>List of entities that have been added or modified.</returns>
    public IList<TEntity> GetModified()
    {
        return [.. _context.ChangeTracker.Entries<TEntity>()
            .Where(x => x.State == EntityState.Added || x.State == EntityState.Modified)
            .Select(x => x.Entity)];
    }

    #endregion

    #region Add

    /// <summary>
    /// Adds an entity to the repository.
    /// </summary>
    /// <param name="entity">Entity to add to repository.</param>
    public virtual void Add(TEntity entity)
    {
        _context.Add(entity);
    }

    /// <summary>
    /// Adds a collection of entities to the repository.
    /// </summary>
    /// <param name="entities">Collection of entities to add.</param>
    public virtual void AddRange(IEnumerable<TEntity> entities)
    {
        _context.AddRange(entities);
    }

    /// <summary>
    /// Sets an entities state as modified to force it to be updated.
    /// </summary>
    /// <param name="entity">Entity to update in the repository.</param>
    public virtual void Update(TEntity entity)
    {
        _context.Entry(entity).State = EntityState.Modified;
    }

    #endregion

    #region Remove

    /// <summary>
    /// Removes an entity from the repository.
    /// </summary>
    /// <param name="entity">Entity to remove from repository.</param>
    public virtual void Remove(TEntity entity)
    {
        _context.Remove(entity);
    }

    /// <summary>
    /// Removes a collection of entities from the repository.
    /// </summary>
    /// <param name="entities">Collection of entities to remove.</param>
    public virtual void RemoveRange(IEnumerable<TEntity> entities)
    {
        _context.RemoveRange(entities);
    }
    #endregion
}
