using PersistenceNet.Interfaces;
using PersistenceNet.Structs;
using System.Linq.Expressions;

namespace PersistenceNet.Repositorys
{
    public abstract class RepositoryBase<TContext, IElement>(TContext persistenceContext)
        : IRepositoryBase<IElement> where TContext : IDatabaseContext where IElement : class
    {
        protected TContext Context = persistenceContext;

        public abstract Task<OperationReturn> CreateOrUpdateAsync(IElement element);
        public abstract Task<OperationReturn> CreateAsync(IElement element);
        public abstract Task<OperationReturn> UpdateAsync(IElement element);
        public abstract Task<OperationReturn> DeleteAsync(IElement element);
        public abstract Task<OperationReturn> UpdateList(IElement mainElement);
        public abstract void EntityHierarchy(IElement element);

        public abstract Task<IEnumerable<IElement>> Filter(Expression<Func<IElement, bool>> filter, params Expression<Func<IElement, object>>[] includes);
        public abstract Task<IEnumerable<IElement>> Filter(Expression<Func<IElement, bool>> filter);
        public abstract Task<IEnumerable<IElement>> Filter(Expression<Func<IElement, bool>> filter, int pageNumber, int pageSize, params Expression<Func<IElement, object>>[] includes);
        public abstract Task<IEnumerable<IElement>> Filter(Expression<Func<IElement, bool>> filter, int pageNumber, int pageSize);
        public abstract Task<IEnumerable<IElement>> Paginate(int pageNumber, int pageSize, params Expression<Func<IElement, object>>[] includes);
        public abstract Task<IEnumerable<IElement>> Paginate(int pageNumber, int pageSize);

        public abstract Task<IElement> GetEntityTrackingByIdAsync(long id);
        public abstract Task<IElement> GetEntityByIdAsync(long id);
        public abstract Task<IElement> GetEntityByIdAsync(long id, params Expression<Func<IElement, object>>[] includes);
    }
}