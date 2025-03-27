using Flunt.Notifications;
using PersistenceNet.Interfaces;
using PersistenceNet.Structs;
using System.Linq.Expressions;

namespace PersistenceNet.Services
{
    public class ServiceBase<IElement>(IRepositoryBase<IElement> repository)
         : Notifiable<Notification>, IServiceBase<IElement> where IElement : class
    {
        public virtual async Task<OperationReturn> CreateOrUpdateAsync(IElement element)
        {
            return await repository.CreateOrUpdateAsync(element);
        }

        public virtual async Task<OperationReturn> CreateAsync(IElement element)
        {
            return await repository.CreateAsync(element);
        }

        public virtual async Task<OperationReturn> UpdateAsync(IElement element)
        {
            return await repository.UpdateAsync(element);
        }

        public virtual async Task<OperationReturn> UpdateList(IElement mainElement)
        {
            return await repository.UpdateList(mainElement);
        }

        public virtual async Task<OperationReturn> DeleteAsync(IElement element)
        {
            return await repository.DeleteAsync(element);
        }

        public virtual void EntityHierarchy(IElement element)
        {
            repository.EntityHierarchy(element);
        }

        public async Task<IEnumerable<IElement>> Filter(Expression<Func<IElement, bool>> filter, params Expression<Func<IElement, object>>[] includes)
        {
            return await repository.Filter(filter, includes);
        }

        public async Task<IEnumerable<IElement>> Filter(Expression<Func<IElement, bool>> filter)
        {
            return await repository.Filter(filter);
        }

        public Task<IEnumerable<IElement>> Filter(Expression<Func<IElement, bool>> filter, int pageNumber, int pageSize, params Expression<Func<IElement, object>>[] includes)
        {
            return repository.Filter(filter, pageNumber, pageSize, includes);
        }

        public Task<IEnumerable<IElement>> Filter(Expression<Func<IElement, bool>> filter, int pageNumber, int pageSize)
        {
            return repository.Filter(filter, pageNumber, pageSize); 
        }

        public Task<IEnumerable<IElement>> Paginate(int pageNumber, int pageSize, params Expression<Func<IElement, object>>[] includes)
        {
            return repository.Paginate(pageNumber, pageSize, includes);
        }

        public Task<IEnumerable<IElement>> Paginate(int pageNumber, int pageSize)
        {
            return repository.Paginate(pageNumber, pageSize);
        }

        public Task<IElement> GetEntityByIdAsync(long id)
        {
            return repository.GetEntityByIdAsync(id);
        }

        public Task<IElement> GetEntityByIdAsync(long id, params Expression<Func<IElement, object>>[] includes)
        {
            return repository.GetEntityByIdAsync(id, includes);
        }

        public Task<IElement> GetEntityTrackingByIdAsync(long id)
        {
            return repository.GetEntityTrackingByIdAsync(id);
        }
    }
}