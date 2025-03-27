using Flunt.Notifications;
using PersistenceNet.Entitys;
using PersistenceNet.Interfaces;
using PersistenceNet.Structs;
using System.Linq.Expressions;

namespace PersistenceNet.Services
{
    public class ServiceBase<TEntity>(IRepositoryBase<TEntity> repository)
         : Notifiable<Notification>, IServiceBase<TEntity> where TEntity : EntityBase
    {
        public virtual async Task<OperationReturn> CreateOrUpdateAsync(TEntity element)
        {
            return await repository.CreateOrUpdateAsync(element);
        }

        public virtual async Task<OperationReturn> CreateAsync(TEntity element)
        {
            return await repository.CreateAsync(element);
        }

        public virtual async Task<OperationReturn> UpdateAsync(TEntity element)
        {
            return await repository.UpdateAsync(element);
        }

        public virtual async Task<OperationReturn> UpdateList(TEntity mainElement)
        {
            return await repository.UpdateList(mainElement);
        }

        public virtual async Task<OperationReturn> DeleteAsync(TEntity element)
        {
            return await repository.DeleteAsync(element);
        }

        public virtual void EntityHierarchy(TEntity element)
        {
            repository.EntityHierarchy(element);
        }

        public async Task<IEnumerable<TEntity>> Filter(Expression<Func<TEntity, bool>> filter
            , params Expression<Func<TEntity, object>>[] includes)
        {
            return await repository.Filter(filter, includes);
        }

        public async Task<IEnumerable<TEntity>> Filter(Expression<Func<TEntity, bool>> filter)
        {
            return await repository.Filter(filter);
        }

        public Task<IEnumerable<TEntity>> Filter(Expression<Func<TEntity, bool>> filter
            , int pageNumber, int pageSize, params Expression<Func<TEntity, object>>[] includes)
        {
            return repository.Filter(filter, pageNumber, pageSize, includes);
        }

        public Task<IEnumerable<TEntity>> Filter(Expression<Func<TEntity, bool>> filter, int pageNumber, int pageSize)
        {
            return repository.Filter(filter, pageNumber, pageSize); 
        }

        public Task<IEnumerable<TEntity>> Paginate(int pageNumber, int pageSize
            , params Expression<Func<TEntity, object>>[] includes)
        {
            return repository.Paginate(pageNumber, pageSize, includes);
        }

        public Task<IEnumerable<TEntity>> Paginate(int pageNumber, int pageSize)
        {
            return repository.Paginate(pageNumber, pageSize);
        }

        public Task<TEntity> GetEntityByIdAsync(long id)
        {
            return repository.GetEntityByIdAsync(id);
        }

        public Task<TEntity> GetEntityByIdAsync(long id, params Expression<Func<TEntity, object>>[] includes)
        {
            return repository.GetEntityByIdAsync(id, includes);
        }

        public Task<TEntity> GetEntityTrackingByIdAsync(long id)
        {
            return repository.GetEntityTrackingByIdAsync(id);
        }
    }
}