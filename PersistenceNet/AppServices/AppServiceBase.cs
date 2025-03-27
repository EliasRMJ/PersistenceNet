using Microsoft.Extensions.Logging;
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
                                             , ILogger<T> logger)
        : IAppServiceBase<T> where T : class where P : class
    {
        public virtual async Task<OperationReturn> CreateOrUpdateAsync(T element)
        {
            logger.LogInformation("Starting the Object to Entity Conversion in the Method CreateOrUpdate.");
            P convertedEntity = mapper.Map<P>(element);

            logger.LogInformation("Converting the Object to the Entity Successfully Performed in the Method CreateOrUpdate.");
            return await serviceBase.CreateOrUpdateAsync(convertedEntity);
        }

        public virtual async Task<OperationReturn> CreateAsync(T element)
        {
            logger.LogInformation("Starting the Object to Entity Conversion in the Method Create.");
            P convertedEntity = mapper.Map<P>(element);

            logger.LogInformation("Converting the Object to the Entity Successfully Performed in the Method Create.");
            return await serviceBase.CreateAsync(convertedEntity);
        }

        public virtual async Task<OperationReturn> UpdateAsync(T element)
        {
            logger.LogInformation("Starting the Object to Entity Conversion in the Method Update.");
            P convertedEntity = mapper.Map<P>(element);

            logger.LogInformation("Converting the Object to the Entity Successfully Performed in the Method Update.");
            return await serviceBase.UpdateAsync(convertedEntity);
        }

        public virtual async Task<OperationReturn> DeleteAsync(T element)
        {
            logger.LogInformation("Starting the Object to Entity Conversion in the Method Delete.");
            P convertedEntity = mapper.Map<P>(element);

            logger.LogInformation("Converting the Object to the Entity Successfully Performed in the Method Delete.");
            return await serviceBase.DeleteAsync(convertedEntity);
        }

        public virtual async Task<IEnumerable<T>> Filter(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes)
        {
            var filterConvert = ExpressionFuncConvert.Builder<T, P>(filter);
            var includesConvert = ExpressionFuncConvert.ConvertIncludesExpression<T, P>(includes);
            var resultList = await serviceBase.Filter(filterConvert, includesConvert);

            if (resultList is null || !resultList.Any())
                throw new Exception("Nenhum registro encontrado no momento com os filtros informados.");

            return mapper.Map<IEnumerable<T>>(resultList);
        }

        public virtual async Task<IEnumerable<T>> Filter(Expression<Func<T, bool>> filter)
        {
            var filterConvert = ExpressionFuncConvert.Builder<T, P>(filter);
            var resultList = await serviceBase.Filter(filterConvert);

            if (resultList is null || !resultList.Any())
                throw new Exception("Nenhum registro encontrado no momento com os filtros informados.");

            return mapper.Map<IEnumerable<T>>(resultList);
        }

        public virtual async Task<IEnumerable<T>> Filter(Expression<Func<T, bool>> filter, int pageNumber, int pageSize, params Expression<Func<T, object>>[] includes)
        {
            var filterConvert = ExpressionFuncConvert.Builder<T, P>(filter);
            var includesConvert = ExpressionFuncConvert.ConvertIncludesExpression<T, P>(includes);
            var resultList = await serviceBase.Filter(filterConvert, pageNumber, pageSize, includesConvert);

            if (resultList is null || !resultList.Any())
                throw new Exception("Nenhum registro encontrado no momento com os filtros informados.");

            return mapper.Map<IEnumerable<T>>(resultList);
        }

        public virtual async Task<IEnumerable<T>> Filter(Expression<Func<T, bool>> filter, int pageNumber, int pageSize)
        {
            var filterConvert = ExpressionFuncConvert.Builder<T, P>(filter);
            var resultList = await serviceBase.Filter(filterConvert, pageNumber, pageSize);

            if (resultList is null || !resultList.Any())
                throw new Exception("Nenhum registro encontrado no momento com os filtros informados.");

            return mapper.Map<IEnumerable<T>>(resultList);
        }

        public virtual async Task<IEnumerable<T>> Paginate(int pageNumber, int pageSize, params Expression<Func<T, object>>[] includes)
        {
            var includesConvert = ExpressionFuncConvert.ConvertIncludesExpression<T, P>(includes);
            var resultList = await serviceBase.Paginate(pageNumber, pageSize, includesConvert);

            if (resultList is null || !resultList.Any())
                throw new Exception("Nenhum registro encontrado no momento.");

            return mapper.Map<IEnumerable<T>>(resultList);
        }

        public virtual async Task<IEnumerable<T>> Paginate(int pageNumber, int pageSize)
        {
            var resultList = await serviceBase.Paginate(pageNumber, pageSize);

            if (resultList is null || !resultList.Any())
                throw new Exception("Nenhum registro encontrado no momento.");

            return mapper.Map<IEnumerable<T>>(resultList);
        }

        public virtual async Task<T> GetEntityByIdAsync(long id)
        {
            var entity = await serviceBase.GetEntityByIdAsync(id);
            return entity is null
                ? throw new Exception($"Registro com identificador '{id}' não encontrado no momento.")
                : mapper.Map<T>(entity);
        }

        public virtual async Task<T> GetEntityByIdAsync(long id, params Expression<Func<T, object>>[] includes)
        {
            var includesConvert = ExpressionFuncConvert.ConvertIncludesExpression<T, P>(includes);
            var entity = await serviceBase.GetEntityByIdAsync(id, includesConvert);

            return entity is null
                ? throw new Exception($"Registro com identificador '{id}' não encontrado no momento.")
                : mapper.Map<T>(entity);
        }
    }
}