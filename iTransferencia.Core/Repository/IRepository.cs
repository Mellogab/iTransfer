using iTransferencia.Core.Entities;
using System.Linq.Expressions;

namespace iTransferencia.Core.Repository
{
    public interface IRepository<TEntity, TPrimaryKey> where TEntity : Entity<TPrimaryKey>
    {
        Task AddAsync(TEntity entity);
        Task AddCollectionAsync(IEnumerable<TEntity> entities);
        IEnumerable<TEntity> AddCollectionWithProxy(IEnumerable<TEntity> entities);
        IQueryable<TEntity> GetAll(bool asNoTracking = true);
        IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> where, bool asNoTracking = true);
        TEntity GetSingle(Expression<Func<TEntity, bool>> where, bool asNoTracking = true);
        IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, object>> orderBy, bool asNoTracking = true);
        Task<IEnumerable<TEntity>> GetAllAsync(bool asNoTracking = true);
        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> where, bool asNoTracking = true);
        Task<TEntity> GetSingleAsync(Expression<Func<TEntity, bool>> where, bool asNoTracking = true);
        Task<IEnumerable<TEntity>> GetAllAsync(Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, object>> orderBy, bool asNoTracking = true);
        Task<Tuple<IEnumerable<TEntity>, int>> GetAll(int skip, int take, bool asNoTracking = true);
        Task<Tuple<IEnumerable<TEntity>, int>> GetAll(int skip, int take, Expression<Func<TEntity, bool>> where, bool asNoTracking = true);
        Task<Tuple<IEnumerable<TEntity>, int>> GetAll(int skip, int take, Expression<Func<TEntity, bool>> where, Expression<Func<TEntity, object>> orderBy, bool asNoTracking = true);
        TEntity GetById(TPrimaryKey entityId, bool asNoTracking = true);
        Task<TEntity> GetByIdAsync(TPrimaryKey entityId, bool asNoTracking = true);
        Task RemoveAsync(TEntity entity);
        Task RemoveByAsync(Func<TEntity, bool> where);
        Task SaveChangesAsync();
        Task UpdateAsync(TEntity entity);
        Task UpdateCollectionAsync(IEnumerable<TEntity> entities);
        IEnumerable<TEntity> UpdateCollectionWithProxy(IEnumerable<TEntity> entities);
    }
}
