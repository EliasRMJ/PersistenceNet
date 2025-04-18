﻿using Microsoft.Extensions.Logging;
using PersistenceNet.Interfaces;
using AutoMapper;
using PersistenceNet.Structs;
using System.Linq.Expressions;
using PersistenceNet.Utils;

namespace PersistenceNet.AppServices
{
    public abstract class AppServiceBase<T, P>(IServiceBase<P> serviceBase
                                             , ITransactionWork transactionWork
                                             , IMapper mapper
                                             , ILogger<T> logger
                                             , IMessagesProvider provider)
        : IAppServiceBase<T> where T : class where P : class
    {
        public virtual async Task<OperationReturn> CreateOrUpdateAsync(T element)
        {
            logger.LogInformation($"{provider.Current.EntityConversion} 'CreateOrUpdate'.");
            P convertedEntity = mapper.Map<P>(element);

            logger.LogInformation($"{provider.Current.StartCallMethod} 'CreateOrUpdate'.");
            return await serviceBase.CreateOrUpdateAsync(convertedEntity);
        }

        public virtual async Task<OperationReturn> CreateAsync(T element)
        {
            logger.LogInformation($"{provider.Current.EntityConversion} 'Create'.");
            P convertedEntity = mapper.Map<P>(element);

            logger.LogInformation($"{provider.Current.StartCallMethod} 'Create'.");
            return await serviceBase.CreateAsync(convertedEntity);
        }

        public virtual async Task<OperationReturn> UpdateAsync(T element)
        {
            logger.LogInformation($"{provider.Current.EntityConversion} 'Update'.");
            P convertedEntity = mapper.Map<P>(element);

            logger.LogInformation($"{provider.Current.StartCallMethod} 'Update'.");
            return await serviceBase.UpdateAsync(convertedEntity);
        }

        public virtual async Task<OperationReturn> DeleteAsync(T element)
        {
            logger.LogInformation($"{provider.Current.EntityConversion} 'Delete'.");
            P convertedEntity = mapper.Map<P>(element);

            logger.LogInformation($"{provider.Current.StartCallMethod} 'Delete'.");
            return await serviceBase.DeleteAsync(convertedEntity);
        }

        public virtual async Task<IEnumerable<T>> Filter(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes)
        {
            var filterConvert = ExpressionFuncConvert.Builder<T, P>(filter);
            var includesConvert = ExpressionFuncConvert.ConvertIncludesExpression<T, P>(includes);
            var resultList = await serviceBase.Filter(filterConvert, includesConvert);

            if (resultList is null || !resultList.Any())
                throw new Exception($"{provider.Current.NoResultList}");

            return mapper.Map<IEnumerable<T>>(resultList);
        }

        public virtual async Task<IEnumerable<T>> Filter(Expression<Func<T, bool>> filter)
        {
            var filterConvert = ExpressionFuncConvert.Builder<T, P>(filter);
            var resultList = await serviceBase.Filter(filterConvert);

            if (resultList is null || !resultList.Any())
                throw new Exception($"{provider.Current.NoResultList}");

            return mapper.Map<IEnumerable<T>>(resultList);
        }

        public virtual async Task<IEnumerable<T>> Filter(Expression<Func<T, bool>> filter, int pageNumber, int pageSize, params Expression<Func<T, object>>[] includes)
        {
            var filterConvert = ExpressionFuncConvert.Builder<T, P>(filter);
            var includesConvert = ExpressionFuncConvert.ConvertIncludesExpression<T, P>(includes);
            var resultList = await serviceBase.Filter(filterConvert, pageNumber, pageSize, includesConvert);

            if (resultList is null || !resultList.Any())
                throw new Exception($"{provider.Current.NoResultList}");

            return mapper.Map<IEnumerable<T>>(resultList);
        }

        public virtual async Task<IEnumerable<T>> Filter(Expression<Func<T, bool>> filter, int pageNumber, int pageSize)
        {
            var filterConvert = ExpressionFuncConvert.Builder<T, P>(filter);
            var resultList = await serviceBase.Filter(filterConvert, pageNumber, pageSize);

            if (resultList is null || !resultList.Any())
                throw new Exception($"{provider.Current.NoResultList}");

            return mapper.Map<IEnumerable<T>>(resultList);
        }

        public virtual async Task<IEnumerable<T>> Paginate(int pageNumber, int pageSize, params Expression<Func<T, object>>[] includes)
        {
            var includesConvert = ExpressionFuncConvert.ConvertIncludesExpression<T, P>(includes);
            var resultList = await serviceBase.Paginate(pageNumber, pageSize, includesConvert);

            if (resultList is null || !resultList.Any())
                throw new Exception($"{provider.Current.NoResultList}");

            return mapper.Map<IEnumerable<T>>(resultList);
        }

        public virtual async Task<IEnumerable<T>> Paginate(int pageNumber, int pageSize)
        {
            var resultList = await serviceBase.Paginate(pageNumber, pageSize);

            if (resultList is null || !resultList.Any())
                throw new Exception($"{provider.Current.NoResultList}");

            return mapper.Map<IEnumerable<T>>(resultList);
        }

        public virtual async Task<T> GetEntityByIdAsync(long id)
        {
            var entity = await serviceBase.GetEntityByIdAsync(id);
            return entity is null
                ? throw new Exception($"[{id}] {provider.Current.NoResult}")
                : mapper.Map<T>(entity);
        }

        public virtual async Task<T> GetEntityByIdAsync(long id, params Expression<Func<T, object>>[] includes)
        {
            var includesConvert = ExpressionFuncConvert.ConvertIncludesExpression<T, P>(includes);
            var entity = await serviceBase.GetEntityByIdAsync(id, includesConvert);

            return entity is null
                ? throw new Exception($"[{id}] {provider.Current.NoResult}")
                : mapper.Map<T>(entity);
        }
    }
}