using System.Data.SqlClient;
using System.Linq.Expressions;
using Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Data.Providers
{
    public class Repository<TEntity, TKey>(ApplicationDbContext context) : IRepository<TEntity, TKey>
        where TEntity : class
    {
        private readonly DbSet<TEntity> _entity = context.Set<TEntity>();


        public async Task<TEntity?> GetByIdAsync(TKey id, CancellationToken cancellationToken)
        {
            return await _entity.FindAsync(id, cancellationToken);
        }

        public async Task<IEnumerable<TEntity>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _entity.ToListAsync(cancellationToken);
        }

        public Task<IEnumerable<TEntity>> GetAllAsync(params Expression<Func<TEntity, object>>[]? includeProperties)
        {
            return Task.FromResult<IEnumerable<TEntity>>(ApplyIncludesOnQuery(_entity, includeProperties));
        }

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        {
            return await _entity.Where(predicate).ToListAsync(cancellationToken);
        }

        public async Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        {
            return await _entity.FirstOrDefaultAsync(predicate,cancellationToken);
        }
        public async Task<TEntity?> SingleOrDefaultAsync(Expression<Func<TEntity, bool>> predicate,CancellationToken cancellationToken)
        {
            return await _entity.SingleOrDefaultAsync(predicate, cancellationToken);
        }
        public async Task<TEntity?> AddAsync(TEntity entity, CancellationToken cancellationToken)
        {
            await _entity.AddAsync(entity,cancellationToken);
            return entity;
        }

        public async Task<IEnumerable<TEntity>> AddRangeAsync(IEnumerable<TEntity> entities,
            CancellationToken cancellationToken)
        {
            var addRangeAsync = entities as TEntity[] ?? entities.ToArray();
            await _entity.AddRangeAsync(addRangeAsync,cancellationToken);
            return addRangeAsync;
        }

        public async Task<TEntity> Remove(TEntity entity)
        {
            await Task.FromResult(_entity.Remove(entity));
            return entity;
        }

        public Task<IEnumerable<TEntity>> RemoveRange(IEnumerable<TEntity> entities)
        {
            var removeRange = entities as TEntity[] ?? entities.ToArray();
            _entity.RemoveRange(removeRange);
            return Task.FromResult<IEnumerable<TEntity>>(removeRange);
        }

        public async Task Attach(TEntity entity)
        {
            await Task.FromResult(_entity.Attach(entity));
        }


        public Task<TEntity> Update(TEntity entity, params Expression<Func<TEntity, object>>[] propsToBeExcluded)
        {
            _entity.Update(entity);
            return Task.FromResult(entity);
        }
        public Task<IEnumerable<TEntity>> UpdateRange(IEnumerable<TEntity> entities)
        {
            _entity.UpdateRange(entities);
            return Task.FromResult(entities);
        }
        public Task<TAnotherEntity> UpdateMinimal<TAnotherEntity>(TAnotherEntity entity,
            params Expression<Func<TAnotherEntity, object>>[] propsToBeUpdated) where TAnotherEntity : class
        {
            context.Entry(entity).State = EntityState.Modified;
            return Task.FromResult(entity);
        }

        public async Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken)
        {
            return await _entity.AnyAsync(predicate, cancellationToken);
        }

        public Task<TOEntity> RemoveOther<TOEntity>(TOEntity entity) where TOEntity : class
        {
            context.Set<TOEntity>().Remove(entity);
            return Task.FromResult(entity);
        }
        public Task<IEnumerable<TOEntity>> RemoveRangeOther<TOEntity>(IEnumerable<TOEntity> entities) where TOEntity : class
        {
            context.Set<TOEntity>().RemoveRange(entities);
            return Task.FromResult(entities);
        }

        public Task AttachOtherEntity<TOEntity>(TOEntity otherEntity) where TOEntity : class
        {
            context.Set<TOEntity>().Attach(otherEntity);
            return Task.CompletedTask;
        }

        public async Task ExecuteSqlRawAsync(string sql, CancellationToken cancellationToken, params object[] parameters)
        {
            await context.Database.ExecuteSqlRawAsync(sql, cancellationToken, parameters);
        }

        public async Task ExecuteSqlRawAsync(string sql, CancellationToken cancellationToken)
        {
            await context.Database.ExecuteSqlRawAsync(sql,cancellationToken);
        }

        public async Task<IEnumerable<TOther>> ExecuteSqlQueryAsync<TOther>(string sql, CancellationToken cancellationToken, params object[] parameters) where TOther : class
        {
            return await context.Set<TOther>().FromSqlRaw(sql, parameters).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<TOther>> ExecuteSqlQueryAsync<TOther>(string sql, SqlParameter[] parameters, CancellationToken cancellationToken) where TOther : class
        {
            return await context.Set<TOther>().FromSqlRaw(sql, parameters).ToListAsync(cancellationToken);
        }

        public async Task<IEnumerable<TOther>> ExecuteProcedureAsync<TOther>(string procedureName, List<SqlParameter> parameters, CancellationToken cancellationToken) where TOther : class
        {
            var sql = $"EXEC {procedureName} {string.Join(", ", parameters.Select(p => $"@{p.ParameterName}"))}";
            return await context.Set<TOther>().FromSqlRaw(sql, parameters.ToArray()).ToListAsync(cancellationToken);
        }

        public DbSet<TOther> GetEntity<TOther>() where TOther : class
        {
            return context.Set<TOther>();
        }

        public async Task<IEnumerable<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[]? includeProperties)
        {
            var query = _entity.AsQueryable();

            if (predicate != null)
            {
                query = query.Where(predicate).AsQueryable();
            }

            if (includeProperties != null)
            {
                query = ApplyIncludesOnQuery(query, includeProperties);
            }

            return query;
        }

        public async Task<IEnumerable<TEntity>> FindInChunk<TSortedBy>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TSortedBy>> orderBy, int chunkSize = 500, params Expression<Func<TEntity, object>>[]? includeProperties)
        {
            var query = _entity.Where(predicate);

            if (includeProperties != null)
            {
                query = ApplyIncludesOnQuery(query, includeProperties);
            }
            query = query.OrderBy(orderBy);

            return ChunkDataInternal(query, chunkSize).SelectMany(_ => _);
        }

        public void Dispose()
        {
            context.Dispose();
            GC.SuppressFinalize(this);
        }
        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            return await context.SaveChangesAsync(cancellationToken);
        }
        public IQueryable<TEntity> ApplyIncludesOnQuery(IQueryable<TEntity> query, params Expression<Func<TEntity, object>>[]? includeProperties)
        {
            // Return Applied Includes query
            return (includeProperties.Aggregate(query, (current, include) => current.Include(include)));
        }
        private IEnumerable<IEnumerable<TSource>> ChunkDataInternal<TSource>(IQueryable<TSource> source, int chunkSize)
        {
            for (int i = 0; i < source.Count(); i += chunkSize)
                yield return source.Skip(i).Take(chunkSize);
        }
    }

}
