using PersistenceNet.Entitys;
using PersistenceNet.Interfaces;
using PersistenceNet.Structs;
using System.Linq.Expressions;

namespace PersistenceNet.Repositorys
{
    public abstract class RepositoryBase<TContext, TEntity>(TContext persistenceContext)
        : IRepositoryBase<TEntity> where TContext : IDatabaseContext where TEntity : EntityBase
    {
        protected TContext Context = persistenceContext;

        public abstract Task<OperationReturn> CreateOrUpdateAsync(TEntity element);
        public abstract Task<OperationReturn> CreateAsync(TEntity element);
        public abstract Task<OperationReturn> UpdateAsync(TEntity element);
        public abstract Task<OperationReturn> DeleteAsync(TEntity element);
        public abstract Task<OperationReturn> UpdateList(TEntity mainElement);
        public abstract void EntityHierarchy(TEntity element);

        public abstract Task<IEnumerable<TEntity>> Filter(Expression<Func<TEntity, bool>> filter, params Expression<Func<TEntity, object>>[] includes);
        public abstract Task<IEnumerable<TEntity>> Filter(Expression<Func<TEntity, bool>> filter);
        public abstract Task<IEnumerable<TEntity>> Filter(Expression<Func<TEntity, bool>> filter, int pageNumber, int pageSize, params Expression<Func<TEntity, object>>[] includes);
        public abstract Task<IEnumerable<TEntity>> Filter(Expression<Func<TEntity, bool>> filter, int pageNumber, int pageSize);
        public abstract Task<IEnumerable<TEntity>> Paginate(int pageNumber, int pageSize, params Expression<Func<TEntity, object>>[] includes);
        public abstract Task<IEnumerable<TEntity>> Paginate(int pageNumber, int pageSize);

        public abstract Task<TEntity> GetEntityTrackingByIdAsync(long id);
        public abstract Task<TEntity> GetEntityByIdAsync(long id);
        public abstract Task<TEntity> GetEntityByIdAsync(long id, params Expression<Func<TEntity, object>>[] includes);
    }
}