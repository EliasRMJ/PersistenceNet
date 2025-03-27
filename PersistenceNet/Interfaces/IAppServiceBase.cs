using PersistenceNet.Structs;
using System.Linq.Expressions;

namespace PersistenceNet.Interfaces
{
    public interface IAppServiceBase<EntityViewModel> where EntityViewModel : class
    {
        Task<OperationReturn> CreateOrUpdateAsync(EntityViewModel element);
        Task<OperationReturn> CreateAsync(EntityViewModel element);
        Task<OperationReturn> UpdateAsync(EntityViewModel element);
        Task<OperationReturn> DeleteAsync(EntityViewModel element);

        Task<IEnumerable<EntityViewModel>> Filter(Expression<Func<EntityViewModel, bool>> filter, params Expression<Func<EntityViewModel, object>>[] includes);
        Task<IEnumerable<EntityViewModel>> Filter(Expression<Func<EntityViewModel, bool>> filter);
        Task<IEnumerable<EntityViewModel>> Filter(Expression<Func<EntityViewModel, bool>> filter, int pageNumber, int pageSize, params Expression<Func<EntityViewModel, object>>[] includes);
        Task<IEnumerable<EntityViewModel>> Filter(Expression<Func<EntityViewModel, bool>> filter, int pageNumber, int pageSize);
        Task<IEnumerable<EntityViewModel>> Paginate(int pageNumber, int pageSize, params Expression<Func<EntityViewModel, object>>[] includes);
        Task<IEnumerable<EntityViewModel>> Paginate(int pageNumber, int pageSize);

        Task<EntityViewModel> GetEntityByIdAsync(long id);
        Task<EntityViewModel> GetEntityByIdAsync(long id, params Expression<Func<EntityViewModel, object>>[] includes);        
    }
}