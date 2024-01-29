using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using System.Linq.Expressions;

namespace Data.Providers
{
    public interface IRepository<TEntity, in TKey> : IDisposable where TEntity : class
    {
        Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken);
        Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken);
        Task<IEnumerable<TEntity>> GetAllAsync(params Expression<Func<TEntity, object>>[]? includeProperties);
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);
        Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);
        Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);

        Task<TEntity?> AddAsync(TEntity entity, CancellationToken cancellationToken);
        Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities, CancellationToken cancellationToken);

        Task<TEntity> Remove(TEntity entity);
        Task<IEnumerable<TEntity>> RemoveRange(IEnumerable<TEntity> entities);

        Task Attach(TEntity entity);

        Task<TEntity> Update(TEntity entity, params Expression<Func<TEntity, object>>[] propsToBeExcluded);
        Task<IEnumerable<TEntity>> UpdateRange(IEnumerable<TEntity> entities);
        Task<TAnotherEntity> UpdateMinimal<TAnotherEntity>(TAnotherEntity entity,
            params Expression<Func<TAnotherEntity, object>>[] propsToBeUpdated) where TAnotherEntity : class;

        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken);

        Task<TOEntity> RemoveOther<TOEntity>(TOEntity entity) where TOEntity : class;
        Task<IEnumerable<TOEntity>> RemoveRangeOther<TOEntity>(IEnumerable<TOEntity> entities) where TOEntity : class;
        Task AttachOtherEntity<TOEntity>(TOEntity otherEntity) where TOEntity : class;
        Task ExecuteSqlRawAsync(string sql, CancellationToken cancellationToken, params object[] parameters);
        Task ExecuteSqlRawAsync(string sql, CancellationToken cancellationToken);
        Task<IEnumerable<TOther>> ExecuteSqlQueryAsync<TOther>(string sql, CancellationToken cancellationToken, params object[] parameters) where TOther : class;
        Task<IEnumerable<TOther>> ExecuteSqlQueryAsync<TOther>(string sql, SqlParameter[] parameters, CancellationToken cancellationToken) where TOther : class;
        Task<IEnumerable<TOther>> ExecuteProcedureAsync<TOther>(string procedureName, List<SqlParameter> parameters, CancellationToken cancellationToken) where TOther : class;
        DbSet<TOther> GetEntity<TOther>() where TOther : class;
        Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[]? includeProperties);
        Task<IEnumerable<TEntity>> FindInChunk<TSortedBy>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TSortedBy>> orderBy, int chunkSize = 500, params Expression<Func<TEntity, object>>[]? includeProperties);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    }
}
