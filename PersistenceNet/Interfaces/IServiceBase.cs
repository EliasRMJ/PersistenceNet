using PersistenceNet.Structs;
using System.Linq.Expressions;

namespace PersistenceNet.Interfaces
{
    public interface IServiceBase<TEntity> where TEntity : class
    {
        Task<OperationReturn> CreateOrUpdateAsync(TEntity element);
        Task<OperationReturn> CreateAsync(TEntity element);
        Task<OperationReturn> UpdateAsync(TEntity element);
        Task<OperationReturn> DeleteAsync(TEntity element);
        Task<OperationReturn> UpdateList(TEntity mainElement);
    
        Task<IEnumerable<TEntity>> Filter(Expression<Func<TEntity, bool>> filter, params Expression<Func<TEntity, object>>[] includes);
        Task<IEnumerable<TEntity>> Filter(Expression<Func<TEntity, bool>> filter);
        Task<IEnumerable<TEntity>> Filter(Expression<Func<TEntity, bool>> filter, int pageNumber, int pageSize, params Expression<Func<TEntity, object>>[] includes);
        Task<IEnumerable<TEntity>> Filter(Expression<Func<TEntity, bool>> filter, int pageNumber, int pageSize);
        Task<IEnumerable<TEntity>> Paginate(int pageNumber, int pageSize, params Expression<Func<TEntity, object>>[] includes);
        Task<IEnumerable<TEntity>> Paginate(int pageNumber, int pageSize);

        Task<TEntity> GetEntityTrackingByIdAsync(long id);
        Task<TEntity> GetEntityByIdAsync(long id);
        Task<TEntity> GetEntityByIdAsync(long id, params Expression<Func<TEntity, object>>[] includes);
    }
}