using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PersistenceNet.Constants;
using PersistenceNet.Entitys;
using PersistenceNet.Enuns;
using PersistenceNet.Interfaces;
using PersistenceNet.Structs;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

namespace PersistenceNet.Repositorys
{
    public class PersistenceData<TDatabaseContext, TEntity>(TDatabaseContext persistenceContext
                                                          , ILogger<PersistenceData<TDatabaseContext, TEntity>> logger
                                                          , IMessagesProvider provider) 
        : IRepositoryBase<TEntity>
        where TEntity : EntityBase
        where TDatabaseContext : PersistenceContext
    {
        #region Métodos públicos
        public virtual async Task<OperationReturn> CreateOrUpdateAsync(TEntity element)
        {
            logger.LogInformation($"NewOrUpdate: {element.GetType().Name} - {GetKeyValue(element)}");
            return element.ElementStates == ElementStatesEnum.New ?
                await CreateAsync(element) :
                await UpdateAsync(element);
        }

        public virtual async Task<OperationReturn> CreateAsync(TEntity element)
        {
            OperationReturn _return = new() { ReturnType = ReturnTypeEnum.Success };

            if (element == null)
            {
                _return.ReturnType = ReturnTypeEnum.Error;
                _return.Messages.Add(new() { ReturnType = _return.ReturnType, Code = provider.Current.Error, Text = provider.Current.EntityNull });
                return _return;
            }

            _return = await EntityValidation(element);
            if (_return.Messages.Count > 0)
            {
                logger.LogWarning($"CreateAsync: {element.GetType().Name} - {GetKeyValue(element)} - {_return.FormatMessage}");
                return _return;
            }

            _return.EntityName = element.GetType().Name;
            _return.Field = GetKeyName(element);

            try
            {
                await EntityHierarchy(element);

                persistenceContext.Entry(element).State = EntityState.Added;

                logger.LogInformation($"CreateAsync: {element.GetType().Name} - {GetKeyValue(element)} - {_return.EntityName} - SaveChangesAsync");
                int retorno = await persistenceContext.SaveChangesAsync();

                if (retorno > 0)
                {
                    logger.LogInformation($"'{_return.EntityName}' {provider.Current.AddSuccess}");

                    _return.Key = $"{persistenceContext.Entry(element).Entity.Id}";
                    _return.ReturnType = ReturnTypeEnum.Success;
                    _return.Messages.Add(new() { ReturnType = ReturnTypeEnum.Success, Code = provider.Current.Success, Text = $"[{_return.EntityName}] {provider.Current.RegisterSuccess}" });
                }
                else
                {
                    logger.LogWarning($"{provider.Current.IncludeProblem} '{_return.EntityName}'!");
                    _return.ReturnType = ReturnTypeEnum.Error;
                    _return.Messages.Add(new() { ReturnType = ReturnTypeEnum.Error, Code = provider.Current.Error, Text = $"{provider.Current.IncludeProblem} '{_return.EntityName}'!" });
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _return.ReturnType = ReturnTypeEnum.Error;
                _return.Messages.Add(new() { ReturnType = ReturnTypeEnum.Error, Code = provider.Current.Error, Text = ex.Message });

                bool possuiInnerException = ex.InnerException != null;
                var ex2 = ex.InnerException;

                while (possuiInnerException)
                {
                    _return.Messages.Add(new() { ReturnType = ReturnTypeEnum.Error, Code = provider.Current.Error, Text = ex2?.Message });
                    possuiInnerException = ex2?.InnerException != null;
                    ex2 = ex2?.InnerException;
                }

                logger.LogError($"{provider.Current.Error} (01): {_return.FormatMessage}");
            }
            catch (DbUpdateException ex)
            {
                _return.ReturnType = ReturnTypeEnum.Error;
                _return.Messages.Add(new() { ReturnType = ReturnTypeEnum.Error, Code = provider.Current.Error, Text = ex.Message });

                bool possuiInnerException = ex.InnerException != null;
                var ex2 = ex.InnerException;

                while (possuiInnerException)
                {
                    _return.Messages.Add(new() { ReturnType = ReturnTypeEnum.Error, Code = provider.Current.Error, Text = ex2?.Message });
                    possuiInnerException = ex2?.InnerException != null;
                    ex2 = ex2?.InnerException;
                }

                logger.LogError($"{provider.Current.Error} (02): {_return.FormatMessage}");
            }
            catch (Exception ex)
            {
                _return.ReturnType = ReturnTypeEnum.Error;
                _return.Messages.Add(new() { ReturnType = ReturnTypeEnum.Error, Code = provider.Current.Error, Text = ex.Message, Exception = ex });

                bool possuiInnerException = ex.InnerException != null;
                var ex2 = ex.InnerException;

                while (possuiInnerException)
                {
                    _return.Messages.Add(new() { ReturnType = ReturnTypeEnum.Error, Code = provider.Current.Error, Text = ex2?.Message, Exception = ex2 });
                    possuiInnerException = ex2?.InnerException != null;
                    ex2 = ex2?.InnerException;
                }

                logger.LogError($"{provider.Current.Error} (03): {_return.FormatMessage}");
            }

            return _return;
        }

        public virtual async Task<OperationReturn> UpdateAsync(TEntity element)
        {
            OperationReturn _return = new() { ReturnType = ReturnTypeEnum.Success };

            if (element == null)
            {
                _return.ReturnType = ReturnTypeEnum.Error;
                _return.Messages.Add(new() { ReturnType = ReturnTypeEnum.Error, Code = provider.Current.Error, Text = provider.Current.EntityNull });

                return _return;
            }

            _return = await EntityValidation(element);
            if (_return.Messages.Count > 0)
            {
                logger.LogWarning($"UpdateAsync: {element.GetType().Name} - {GetKeyValue(element)} - {_return.FormatMessage}");
                return _return;
            }

            _return.EntityName = element.GetType().Name;
            _return.Key = GetKeyValue(element);
            _return.Field = GetKeyName(element);

            try
            {
                await EntityHierarchy(element);

                persistenceContext.Attach(element);
                persistenceContext.Entry(element).State = EntityState.Modified;

                logger.LogInformation($"UpdateAsync: {element.GetType().Name} - {GetKeyValue(element)} - {_return.EntityName} - SaveChangesAsync");
                var returnSave = await persistenceContext.SaveChangesAsync();

                if (returnSave > 0)
                {
                    logger.LogInformation($"'{_return.EntityName}' {provider.Current.AddSuccess}");
                    _return.ReturnType = ReturnTypeEnum.Success;
                    _return.Messages.Add(new() { ReturnType = ReturnTypeEnum.Success, Code = provider.Current.Success, Text = $"[{_return.EntityName}] {provider.Current.UpdateSuccess}" });
                }
                else
                {
                    logger.LogWarning($"{provider.Current.UpdateProblem} '{_return.EntityName}'!");
                    _return.Messages.Add(new() { ReturnType = ReturnTypeEnum.Error, Code = provider.Current.Error, Text = $"{provider.Current.UpdateProblem} '{_return.EntityName}'!" });
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _return.ReturnType = ReturnTypeEnum.Error;
                _return.Messages.Add(new() { ReturnType = ReturnTypeEnum.Error, Code = provider.Current.Error, Text = ex.Message });

                bool possuiInnerException = ex.InnerException != null;
                var ex2 = ex.InnerException;

                while (possuiInnerException)
                {
                    _return.Messages.Add(new() { ReturnType = ReturnTypeEnum.Error, Code = provider.Current.Error, Text = ex2?.Message });
                    possuiInnerException = ex2?.InnerException != null;
                    ex2 = ex2?.InnerException;
                }

                logger.LogError($"{provider.Current.Error} (01): {_return.FormatMessage}");
            }
            catch (DbUpdateException ex)
            {
                _return.ReturnType = ReturnTypeEnum.Error;
                _return.Messages.Add(new() { ReturnType = ReturnTypeEnum.Error, Code = provider.Current.Error, Text = ex.Message });

                bool possuiInnerException = ex.InnerException != null;
                var ex2 = ex.InnerException;

                while (possuiInnerException)
                {
                    _return.Messages.Add(new() { ReturnType = ReturnTypeEnum.Error, Code = provider.Current.Error, Text = ex2?.Message });
                    possuiInnerException = ex2?.InnerException != null;
                    ex2 = ex2?.InnerException;
                }

                logger.LogError($"{provider.Current.Error} (02): {_return.FormatMessage}");
            }
            catch (Exception ex)
            {
                _return.ReturnType = ReturnTypeEnum.Error;
                _return.Messages.Add(new() { ReturnType = ReturnTypeEnum.Error, Code = provider.Current.Error, Text = ex.Message, Exception = ex });

                bool possuiInnerException = ex.InnerException != null;
                var ex2 = ex.InnerException;

                while (possuiInnerException)
                {
                    _return.Messages.Add(new() { ReturnType = ReturnTypeEnum.Error, Code = provider.Current.Error, Text = ex2?.Message, Exception = ex2 });
                    possuiInnerException = ex2?.InnerException != null;
                    ex2 = ex2?.InnerException;
                }

                logger.LogError($"{provider.Current.Error} (03): {_return.FormatMessage}");
            }

            return _return;
        }

        public virtual async Task<OperationReturn> DeleteAsync(TEntity element)
        {
            OperationReturn _return = new() { ReturnType = ReturnTypeEnum.Success };

            if (element == null)
            {
                _return.ReturnType = ReturnTypeEnum.Error;
                _return.Messages.Add(new() { ReturnType = ReturnTypeEnum.Error, Code = provider.Current.Error, Text = provider.Current.EntityNull });

                return _return;
            }

            var nameEntity = GetDisplayName(element);

            _return.EntityName = element.GetType().Name;
            _return.Key = GetKeyValue(element);
            _return.Field = GetKeyName(element);

            try
            {
                persistenceContext.Remove(element);
                var retorno = await persistenceContext.SaveChangesAsync();

                if (retorno > 0)
                {
                    _return.ReturnType = ReturnTypeEnum.Success;
                    _return.Messages.Add(new() { ReturnType = ReturnTypeEnum.Success, Code = provider.Current.Success, Text = $"[{nameEntity}] {provider.Current.DeleteSuccess}" });
                }
                else
                {
                    _return.ReturnType = ReturnTypeEnum.Error;
                    _return.Messages.Add(new() { ReturnType = ReturnTypeEnum.Error, Code = provider.Current.Error, Text = $"{provider.Current.DeleteProblem} '{nameEntity}'!" });
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _return.ReturnType = ReturnTypeEnum.Error;
                _return.Messages.Add(new() { ReturnType = ReturnTypeEnum.Error, Code = provider.Current.Error, Text = ex.Message });

                bool possuiInnerException = ex.InnerException != null;
                var ex2 = ex?.InnerException;

                while (possuiInnerException)
                {
                    _return.Messages.Add(new() { ReturnType = ReturnTypeEnum.Error, Code = provider.Current.Error, Text = ex2?.Message });
                    possuiInnerException = ex2?.InnerException != null;
                    ex2 = ex2?.InnerException;
                }
                logger.LogError($"{provider.Current.Error} (01): {_return.FormatMessage}");
            }
            catch (DbUpdateException ex)
            {
                _return.ReturnType = ReturnTypeEnum.Error;
                _return.Messages.Add(new() { ReturnType = ReturnTypeEnum.Error, Code = provider.Current.Error, Text = ex.Message });

                bool possuiInnerException = ex.InnerException != null;
                var ex2 = ex.InnerException;

                while (possuiInnerException)
                {
                    _return.Messages.Add(new() { ReturnType = ReturnTypeEnum.Error, Code = provider.Current.Error, Text = ex2?.Message });
                    possuiInnerException = ex2?.InnerException != null;
                    ex2 = ex2?.InnerException;
                }
                logger.LogError($"{provider.Current.Error} (02): {_return.FormatMessage}");
            }
            catch (Exception ex)
            {
                _return.ReturnType = ReturnTypeEnum.Error;
                _return.Messages.Add(new()
                {
                    ReturnType = ReturnTypeEnum.Error,
                    Code = provider.Current.Error,
                    Text = $"{provider.Current.UnexpectedError} '{nameEntity}'."
                });
                _return.Messages.Add(new() { ReturnType = ReturnTypeEnum.Error, Code = provider.Current.Error, Text = ex.Message });

                bool possuiInnerException = ex.InnerException != null;
                var ex2 = ex?.InnerException;

                while (possuiInnerException)
                {
                    _return.Messages.Add(new() { ReturnType = ReturnTypeEnum.Error, Code = provider.Current.Error, Text = ex2?.Message });
                    possuiInnerException = ex2?.InnerException != null;
                    ex2 = ex2?.InnerException;
                }
                logger.LogError($"{provider.Current.Error} (03): {_return.FormatMessage}");
            }

            return _return;
        }

        public virtual async Task<OperationReturn> UpdateList(TEntity mainElement)
        {
            var operationReturn = new OperationReturn { ReturnType = ReturnTypeEnum.Success };

            var entityName = GetDisplayName(mainElement);
            var key = GetKeyValue(mainElement);

            operationReturn.EntityName = entityName;
            operationReturn.Key = key;

            var entityCurrent = persistenceContext.Model.FindEntityType(mainElement.GetType());
            var properties = entityCurrent!.GetProperties();

            foreach (var property in properties)
            {
                logger.LogInformation($"[{property}] {provider.Current.CheckProperty}");
                if (property.IsPrimaryKey() && property.IsForeignKey())
                {
                    var foreignKey = property.GetContainingForeignKeys().FirstOrDefault();
                    var propertyName = foreignKey?.PrincipalEntityType.Name.Split('.').Last();
                    var propertyPrimaryForeignKey = mainElement.GetType().GetProperty(propertyName!);

                    if (propertyPrimaryForeignKey == null)
                        continue;

                    var entityProperty = persistenceContext.Model.FindEntityType(propertyPrimaryForeignKey?.ToString()!.Split(' ')[0]!);
                    var propertiesElement = entityProperty!.GetDeclaredMembers();

                    foreach (var propertyElement in propertiesElement)
                    {
                        if (propertyElement!.PropertyInfo!.IsCollectible)
                        {
                            var elementsDelete = propertyElement.PropertyInfo.GetValue(entityProperty.GetType(), null) as ICollection<TEntity>;
                            if (elementsDelete is not null)
                            {
                                foreach (var elementDelete in elementsDelete)
                                {
                                    if (elementDelete.ElementStates == ElementStatesEnum.Delete)
                                    {
                                        var orMessages = await UpdateList(elementDelete);
                                        if (!orMessages.IsSuccess)
                                        {
                                            operationReturn.ReturnType = orMessages.ReturnType;
                                            foreach (var message in orMessages.Messages)
                                                operationReturn.Messages.Add(message);

                                            return operationReturn;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                var elements = property!.PropertyInfo!.GetValue(mainElement, null) as ICollection<TEntity>;

                if (elements is not null)
                {
                    foreach (var element in elements)
                    {
                        if (element.ElementStates == ElementStatesEnum.Delete)
                        {
                            var returnDelete = await DeleteAsync(element);

                            if (!returnDelete.IsSuccess)
                            {
                                operationReturn.ReturnType = returnDelete.ReturnType;
                                operationReturn.Key = key;
                                operationReturn.EntityName = entityName;
                                operationReturn.Messages.Add(new() { ReturnType = returnDelete.ReturnType, Code = Codes._ERROR, Text = returnDelete.FormatMessage });
                                return operationReturn;
                            }
                        }
                    }
                }
            }

            return operationReturn;
        }

        public virtual async Task EntityHierarchy(TEntity element) 
        {
            await Task.FromResult(true);
        }

        public virtual async Task<IEnumerable<TEntity>> Filter(Expression<Func<TEntity, bool>> filter
           , params Expression<Func<TEntity, object>>[] includes)
        {
            var query = persistenceContext.Set<TEntity>()
                    .AsNoTrackingWithIdentityResolution();

            if (includes is not null && includes.Length > 0)
            {
                foreach (var include in includes)
                    query = query.Include(include);
            }

            return await query.Where(filter)
                    .ToListAsync();
        }

        public virtual async Task<IEnumerable<TEntity>> Filter(Expression<Func<TEntity, bool>> filter)
        {
            return await persistenceContext.Set<TEntity>()
                    .AsNoTrackingWithIdentityResolution()
                    .Where(filter)
                    .ToListAsync();
        }

        public virtual async Task<IEnumerable<TEntity>> Filter(Expression<Func<TEntity, bool>> filter
            , int pageNumber, int pageSize, params Expression<Func<TEntity, object>>[] includes)
        {
            var query = persistenceContext.Set<TEntity>()
                    .AsNoTrackingWithIdentityResolution();

            if (includes is not null && includes.Length > 0)
            {
                foreach (var include in includes)
                    query = query.Include(include);
            }

            return await query.Where(filter)
                    .Skip(pageSize * (pageNumber - 1))
                    .Take(pageSize)
                    .ToListAsync();
        }

        public virtual async Task<IEnumerable<TEntity>> Filter(Expression<Func<TEntity, bool>> filter
            , int pageNumber, int pageSize)
        {
            return await persistenceContext.Set<TEntity>()
                    .AsNoTrackingWithIdentityResolution()
                    .Where(filter)
                    .Skip(pageSize * (pageNumber - 1))
                    .Take(pageSize)
                    .ToListAsync();
        }

        public virtual async Task<IEnumerable<TEntity>> Paginate(int pageNumber, int pageSize
            , params Expression<Func<TEntity, object>>[] includes)
        {
            var query = persistenceContext.Set<TEntity>()
                    .AsNoTrackingWithIdentityResolution();

            if (includes is not null && includes.Length > 0)
            {
                foreach (var include in includes)
                    query = query.Include(include);
            }

            return await query.Skip(pageSize * (pageNumber - 1))
                    .Take(pageSize)
                    .ToListAsync();
        }

        public virtual async Task<IEnumerable<TEntity>> Paginate(int pageNumber, int pageSize)
        {
            var query = persistenceContext.Set<TEntity>()
                    .AsNoTrackingWithIdentityResolution();

            return await query.Skip(pageSize * (pageNumber - 1))
                    .Take(pageSize)
                    .ToListAsync();
        }

        public virtual async Task<TEntity> GetEntityTrackingByIdAsync(long id)
        {
#pragma warning disable CS8603 // Possível retorno de referência nula.
            return await persistenceContext.Set<TEntity>()
                    .AsTracking()
                    .FirstOrDefaultAsync(find => find.Id == id);
#pragma warning restore CS8603 // Possível retorno de referência nula.
        }

        public virtual async Task<TEntity> GetEntityByIdAsync(long id)
        {
#pragma warning disable CS8603 // Possível retorno de referência nula.
            return await persistenceContext.Set<TEntity>()
                    .AsNoTrackingWithIdentityResolution()
                    .FirstOrDefaultAsync(find => find.Id == id);
#pragma warning restore CS8603 // Possível retorno de referência nula.
        }

        public virtual async Task<TEntity> GetEntityByIdAsync(long id, params Expression<Func<TEntity, object>>[] includes)
        {
            var query = persistenceContext.Set<TEntity>()
                .AsNoTrackingWithIdentityResolution();

            if (includes is not null && includes.Length > 0)
            {
                foreach (var include in includes)
                    query = query.Include(include);
            }

#pragma warning disable CS8603 // Possível retorno de referência nula.
            return await query.FirstOrDefaultAsync(find => find.Id == id);
#pragma warning restore CS8603 // Possível retorno de referência nula.
        }
        #endregion

        #region Métodos privados
        private async Task<OperationReturn> EntityValidation(TEntity element)
        {
            var operationReturn = new OperationReturn { ReturnType = ReturnTypeEnum.Warning };

            if (element is null)
            {
                operationReturn.Key = "0";
                operationReturn.EntityName = provider.Current.Unknown;
                operationReturn.ReturnType = ReturnTypeEnum.Error;
                operationReturn.Messages.Add(new()
                {
                    ReturnType = operationReturn.ReturnType,
                    Code = provider.Current.Error,
                    Text = provider.Current.EntityNull
                });

                return operationReturn;
            }

            var entityName = GetDisplayName(element);
            var key = GetKeyValue(element);

            operationReturn.EntityName = entityName;
            operationReturn.Key = key;
            var entityCurrent = persistenceContext.Model.FindEntityType(element.GetType());

            if (entityCurrent is not null)
            {
                var properties = entityCurrent!.GetProperties();

                foreach (var property in properties)
                {
                    if (property.IsPrimaryKey() && property.IsForeignKey())
                    {
                        var foreignKey = property.GetContainingForeignKeys().FirstOrDefault();
                        var propertyName = foreignKey!.PrincipalEntityType.Name.Split('.').Last();
                        var propertyForeignKey = element.GetType().GetProperty(propertyName);
                        if (propertyForeignKey == null)
                            continue;

                        var elementBase = propertyForeignKey.GetValue(element, null) as TEntity;

                        if (elementBase is not null)
                        {
                            var orMessages = await EntityValidation(elementBase);
                            if (!orMessages.IsSuccess)
                            {
                                operationReturn.ReturnType = orMessages.ReturnType;
                                foreach (var message in orMessages.Messages)
                                    operationReturn.Messages.Add(message);
                            }
                        }
                    }

                    var value = property!.PropertyInfo!.GetValue(element, null);

                    if (!property.IsNullable)
                    {
                        if (value is null or (object)"")
                        {
                            if (property.PropertyInfo.CustomAttributes.Any() &&
                                property.PropertyInfo.CustomAttributes.ElementAt(0).NamedArguments.Count > 0)
                            {
                                var argument = property.PropertyInfo.CustomAttributes.ElementAt(0).NamedArguments.ElementAt(0);
                                var message = argument.TypedValue.Value?.ToString();

                                operationReturn.Messages.Add(new()
                                {
                                    ReturnType = ReturnTypeEnum.Warning,
                                    Code = provider.Current.Warning,
                                    Text = message
                                });
                            }
                            else
                            {
                                operationReturn.Messages.Add(new()
                                {
                                    ReturnType = ReturnTypeEnum.Warning,
                                    Code = provider.Current.Warning,
                                    Text = $"'{property.Name}' {provider.Current.PropertyRequired}"
                                });
                            }
                        }
                        else if (property.PropertyInfo.PropertyType == typeof(int) &&
                                  int.Parse(value?.ToString()!) == 0 &&
                                  !property.IsPrimaryKey())
                        {
                            if (property.PropertyInfo.CustomAttributes.Any() &&
                                property.PropertyInfo.CustomAttributes.ElementAt(0).NamedArguments.Count > 0)
                            {
                                var argument = property.PropertyInfo.CustomAttributes.ElementAt(0).NamedArguments.ElementAt(0);

                                if (argument.MemberName == Codes._ERRORMESSAGE)
                                {
                                    var message = argument.TypedValue.Value?.ToString();
                                    operationReturn.Messages.Add(new() { ReturnType = ReturnTypeEnum.Warning, Code = provider.Current.Warning, Text = message });
                                }
                            }
                            else
                            {
                                operationReturn.Messages.Add(new()
                                {
                                    ReturnType = ReturnTypeEnum.Warning,
                                    Code = provider.Current.Warning,
                                    Text = $"'{property.Name}' {provider.Current.PropertyRequired}"
                                });
                            }
                        }
                        else if (property.PropertyInfo.PropertyType == typeof(double) &&
                                  double.Parse(value?.ToString()!) == 0D &&
                                  !property.IsPrimaryKey())
                        {
                            if (property.PropertyInfo.CustomAttributes.Any() &&
                                property.PropertyInfo.CustomAttributes.ElementAt(0).NamedArguments.Count > 0)
                            {
                                var argument = property.PropertyInfo.CustomAttributes.ElementAt(0).NamedArguments.ElementAt(0);

                                if (argument.MemberName == Codes._ERRORMESSAGE)
                                {
                                    var message = argument.TypedValue.Value?.ToString();
                                    operationReturn.Messages.Add(new() { ReturnType = ReturnTypeEnum.Warning, Code = provider.Current.Warning, Text = message });
                                }
                            }
                            else
                            {
                                operationReturn.Messages.Add(new()
                                {
                                    ReturnType = ReturnTypeEnum.Warning,
                                    Code = provider.Current.Warning,
                                    Text = $"'{property.Name}' {provider.Current.PropertyRequired}"
                                });
                            }
                        }
                        else if (property.PropertyInfo.PropertyType == typeof(decimal) &&
                                  decimal.Parse(value?.ToString()!) == 0M &&
                                  !property.IsPrimaryKey())
                        {
                            if (property.PropertyInfo.CustomAttributes.Any() &&
                                property.PropertyInfo.CustomAttributes.ElementAt(0).NamedArguments.Count > 0)
                            {
                                var argument = property.PropertyInfo.CustomAttributes.ElementAt(0).NamedArguments.ElementAt(0);

                                if (argument.MemberName == Codes._ERRORMESSAGE)
                                {
                                    var message = argument.TypedValue.Value?.ToString();
                                    operationReturn.Messages.Add(new() { ReturnType = ReturnTypeEnum.Warning, Code = provider.Current.Warning, Text = message });
                                }
                            }
                            else
                            {
                                operationReturn.Messages.Add(new()
                                {
                                    ReturnType = ReturnTypeEnum.Warning,
                                    Code = provider.Current.Warning,
                                    Text = $"'{property.Name}' {provider.Current.PropertyRequired}"
                                });
                            }
                        }
                        else if (property.PropertyInfo.PropertyType == typeof(bool) &&
                                  bool.Parse(value?.ToString()!) &&
                                  !property.IsPrimaryKey())
                        {
                            if (property.PropertyInfo.CustomAttributes.Any() &&
                                property.PropertyInfo.CustomAttributes.ElementAt(0).NamedArguments.Count > 0)
                            {
                                var argument = property.PropertyInfo.CustomAttributes.ElementAt(0).NamedArguments.ElementAt(0);

                                if (argument.MemberName == Codes._ERRORMESSAGE)
                                {
                                    var message = argument.TypedValue.Value?.ToString();
                                    operationReturn.Messages.Add(new() { ReturnType = ReturnTypeEnum.Warning, Code = provider.Current.Warning, Text = message });
                                }
                            }
                            else
                            {
                                operationReturn.Messages.Add(new()
                                {
                                    ReturnType = ReturnTypeEnum.Warning,
                                    Code = provider.Current.Warning,
                                    Text = $"'{property.Name}' {provider.Current.PropertyRequired}"
                                });
                            }
                        }
                        else if (property.PropertyInfo.PropertyType == typeof(float) &&
                                  float.Parse(value?.ToString()!) == 0F &&
                                  !property.IsPrimaryKey())
                        {
                            if (property.PropertyInfo.CustomAttributes.Any() &&
                                property.PropertyInfo.CustomAttributes.ElementAt(0).NamedArguments.Count > 0)
                            {
                                var argument = property.PropertyInfo.CustomAttributes.ElementAt(0).NamedArguments.ElementAt(0);

                                if (argument.MemberName == Codes._ERRORMESSAGE)
                                {
                                    var message = argument.TypedValue.Value?.ToString();
                                    operationReturn.Messages.Add(new() { ReturnType = ReturnTypeEnum.Warning, Code = provider.Current.Warning, Text = message });
                                }
                            }
                            else
                            {
                                operationReturn.Messages.Add(new()
                                {
                                    ReturnType = ReturnTypeEnum.Warning,
                                    Code = provider.Current.Warning,
                                    Text = $"'{property.Name}' {provider.Current.PropertyRequired}"
                                });
                            }
                        }
                        else if (property.PropertyInfo.PropertyType == typeof(long) &&
                                  long.Parse(value?.ToString()!) == 0 &&
                                  !property.IsPrimaryKey())
                        {
                            if (property.PropertyInfo.CustomAttributes.Any() &&
                                property.PropertyInfo.CustomAttributes.ElementAt(0).NamedArguments.Count > 0)
                            {
                                var argument = property.PropertyInfo.CustomAttributes.ElementAt(0).NamedArguments.ElementAt(0);

                                if (argument.MemberName == Codes._ERRORMESSAGE)
                                {
                                    var message = argument.TypedValue.Value?.ToString();
                                    operationReturn.Messages.Add(new() { ReturnType = ReturnTypeEnum.Warning, Code = provider.Current.Warning, Text = message });
                                }
                            }
                            else
                            {
                                operationReturn.Messages.Add(new()
                                {
                                    ReturnType = ReturnTypeEnum.Warning,
                                    Code = provider.Current.Warning,
                                    Text = $"'{property.Name}' {provider.Current.PropertyRequired}"
                                });
                            }
                        }
                        else if (property.PropertyInfo.PropertyType == typeof(DateOnly) &&
                                  DateOnly.Parse(value?.ToString()!) == default &&
                                  !property.IsPrimaryKey())
                        {
                            if (property.PropertyInfo.CustomAttributes.Any() &&
                                property.PropertyInfo.CustomAttributes.ElementAt(0).NamedArguments.Count > 0)
                            {
                                var argument = property.PropertyInfo.CustomAttributes.ElementAt(0).NamedArguments.ElementAt(0);

                                if (argument.MemberName == Codes._ERRORMESSAGE)
                                {
                                    var message = argument.TypedValue.Value?.ToString();
                                    operationReturn.Messages.Add(new() { ReturnType = ReturnTypeEnum.Warning, Code = provider.Current.Warning, Text = message });
                                }
                            }
                            else
                            {
                                operationReturn.Messages.Add(new()
                                {
                                    ReturnType = ReturnTypeEnum.Warning,
                                    Code = provider.Current.Warning,
                                    Text = $"'{property.Name}' {provider.Current.PropertyRequired}"
                                });
                            }
                        }
                        else if (property.PropertyInfo.PropertyType == typeof(DateTime) &&
                                  DateTime.Parse(value?.ToString()!) == default &&
                                  !property.IsPrimaryKey())
                        {
                            if (property.PropertyInfo.CustomAttributes.Any() &&
                                property.PropertyInfo.CustomAttributes.ElementAt(0).NamedArguments.Count > 0)
                            {
                                var argument = property.PropertyInfo.CustomAttributes.ElementAt(0).NamedArguments.ElementAt(0);

                                if (argument.MemberName == Codes._ERRORMESSAGE)
                                {
                                    var message = argument.TypedValue.Value?.ToString();
                                    operationReturn.Messages.Add(new() { ReturnType = ReturnTypeEnum.Warning, Code = provider.Current.Warning, Text = message });
                                }
                            }
                            else
                            {
                                operationReturn.Messages.Add(new()
                                {
                                    ReturnType = ReturnTypeEnum.Warning,
                                    Code = provider.Current.Warning,
                                    Text = $"'{property.Name}' {provider.Current.PropertyRequired}"
                                });
                            }
                        }
                    }

                    if (value != null && property.PropertyInfo.PropertyType == typeof(string))
                    {
                        var maxResult = GetMaxLengthAttributeForPropertie(property.PropertyInfo);
                        if (maxResult.Count > 0)
                        {
                            var maxValue = value!.ToString()!.Length;
                            var maxLenProperty = maxResult.ElementAt(0).Value;
                            if (maxValue > maxLenProperty)
                            {
                                var argument1 = property.PropertyInfo.CustomAttributes.ElementAt(0).NamedArguments.ElementAt(0);
                                if (argument1.MemberName == Codes._ERRORMESSAGE)
                                {
                                    var message = argument1.TypedValue.Value?.ToString();
                                    operationReturn.Messages.Add(new() { ReturnType = ReturnTypeEnum.Warning, Code = provider.Current.Warning, Text = message });
                                }

                                var argument2 = property.PropertyInfo.CustomAttributes.ElementAt(1).NamedArguments.ElementAt(0);
                                if (argument2.MemberName == Codes._ERRORMESSAGE)
                                {
                                    var message = argument2.TypedValue.Value?.ToString();
                                    operationReturn.Messages.Add(new() { ReturnType = ReturnTypeEnum.Warning, Code = provider.Current.Warning, Text = message });
                                }
                            }
                        }
                    }
                }
            }
            else
            {
                operationReturn.Messages.Add(new()
                {
                    ReturnType = ReturnTypeEnum.Warning,
                    Code = provider.Current.Warning,
                    Text = $"[{element.GetType()}] {provider.Current.EntityFound}"
                });
            }

            return await Task.FromResult(operationReturn);
        }

        private static Dictionary<string, int> GetMaxLengthAttributeForPropertie(PropertyInfo propertyInfo)
        {
            var results = new Dictionary<string, int>();
            var maxLength = propertyInfo.GetCustomAttribute<MaxLengthAttribute>()?.Length;
            if (maxLength != null)
                results.Add(propertyInfo.Name, (int)maxLength);
            
            return results;
        }

        private string GetKeyValue(TEntity element)
        {
            if (element is null) return string.Empty;

            var entityType = persistenceContext.Model.FindEntityType(element.GetType());
            if (entityType == null) return string.Empty;
            
            var keysName = entityType!.FindPrimaryKey()!.Properties.Select(x => x.Name);
            var key = string.Empty;
            
            foreach (var keyValuePair in keysName)
                key += $"{element!.GetType()!.GetProperty(keyValuePair!)!.GetValue(element, null)} | ";

            return !string.IsNullOrEmpty(key) ? key[..^2] : string.Empty;
        }

        private string GetKeyName(TEntity element)
        {
            if (element is null) return string.Empty;

            var entityType = persistenceContext.Model.FindEntityType(element?.GetType()!);
            if (entityType == null) return string.Empty;
            
            var keysName = entityType.FindPrimaryKey()!.Properties.Select(x => x.Name);
            var keyNames = string.Empty;
            
            foreach (var keyValuePair in keysName)
                keyNames += $"{keyValuePair} | ";

            return !string.IsNullOrEmpty(keyNames) ? keyNames[..^2] : string.Empty;
        }

        private string GetDisplayName(TEntity element)
        {
            if (element is null) return string.Empty;

            var entityType = persistenceContext.Model.FindEntityType(element?.GetType()!);
            if (entityType == null) return string.Empty;

            var tableName = entityType.ConstructorBinding!.RuntimeType.CustomAttributes?.ElementAt(1).ConstructorArguments?.ElementAt(0).Value;

            return tableName?.ToString()!;
        }
        #endregion
    }
}