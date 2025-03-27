using PersistenceNet.Structs;
using System.Linq.Expressions;

namespace PersistenceNet.Interfaces
{
    public interface IRepositoryBase<IElement> where IElement : class
    {
        Task<OperationReturn> CreateOrUpdateAsync(IElement element);
        Task<OperationReturn> CreateAsync(IElement element);
        Task<OperationReturn> UpdateAsync(IElement element);
        Task<OperationReturn> DeleteAsync(IElement element);
        Task<OperationReturn> UpdateList(IElement mainElement);
        void EntityHierarchy(IElement element);

        Task<IEnumerable<IElement>> Filter(Expression<Func<IElement, bool>> filter, params Expression<Func<IElement, object>>[] includes);
        Task<IEnumerable<IElement>> Filter(Expression<Func<IElement, bool>> filter);
        Task<IEnumerable<IElement>> Filter(Expression<Func<IElement, bool>> filter, int pageNumber, int pageSize, params Expression<Func<IElement, object>>[] includes);
        Task<IEnumerable<IElement>> Filter(Expression<Func<IElement, bool>> filter, int pageNumber, int pageSize);
        Task<IEnumerable<IElement>> Paginate(int pageNumber, int pageSize, params Expression<Func<IElement, object>>[] includes);
        Task<IEnumerable<IElement>> Paginate(int pageNumber, int pageSize);

        Task<IElement> GetEntityTrackingByIdAsync(long id);
        Task<IElement> GetEntityByIdAsync(long id);
        Task<IElement> GetEntityByIdAsync(long id, params Expression<Func<IElement, object>>[] includes);
    }
}