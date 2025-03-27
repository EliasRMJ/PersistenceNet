using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PersistenceNet.Constants;
using PersistenceNet.Enuns;
using PersistenceNet.Interfaces;
using PersistenceNet.Structs;
using System.ComponentModel.DataAnnotations;
using System.Linq.Expressions;
using System.Reflection;

namespace PersistenceNet.Repositorys
{
    public class PersistenceData<TDatabaseContext, IElement>(TDatabaseContext persistenceContext
                                                           , ILogger<PersistenceData<TDatabaseContext, IElement>> logger) 
        : IRepositoryBase<IElement>
        where TDatabaseContext : PersistenceContext
    {
        #region Métodos públicos
        public virtual async Task<OperationReturn> CreateOrUpdateAsync(IElement element)
        {
            logger.LogInformation($"NewOrUpdate: {element.GetType().Name} - {GetKeyValue(element)}");
            return element.ElementStates == ElementStatesEnum.New ?
                await CreateAsync(element) :
                await UpdateAsync(element);
        }

        public virtual async Task<OperationReturn> CreateAsync(IElement element)
        {
            OperationReturn _return = new() { ReturnType = ReturnTypeEnum.Success };

            if (element == null)
            {
                _return.ReturnType = ReturnTypeEnum.Error;
                _return.Messages.Add(new() { ReturnType = _return.ReturnType, Code = Codes._ERROR, Text = "The entity is void!" });
                return _return;
            }

            _return = await EntityValidation(element);
            if (_return.Messages.Count > 0)
            {
                logger.LogWarning($"NewAsync: {element.GetType().Name} - {GetKeyValue(element)} - {_return.FormatMessage}");
                return _return;
            }

            var nameEntity = GetDisplayName(element);

            _return.EntityName = element.GetType().Name;
            _return.Key = GetKeyValue(element);
            _return.Field = GetKeyName(element);

            try
            {
                EntityHierarchy(element);

                persistenceContext.Entry(element).State = EntityState.Added;

                logger.LogInformation($"NewAsync: {element.GetType().Name} - {GetKeyValue(element)} - {nameEntity} - SaveChangesAsync");
                int retorno = await persistenceContext.SaveChangesAsync();

                if (retorno > 0)
                {
                    logger.LogInformation($"'{nameEntity}' successfully added!");
                    _return.ReturnType = ReturnTypeEnum.Success;
                    _return.Messages.Add(new() { ReturnType = ReturnTypeEnum.Success, Code = Codes._SUCCESS, Text = $"'{nameEntity}' successfully added!" });
                }
                else
                {
                    logger.LogWarning($"Ops, something went wrong by including the entity '{nameEntity}'!");
                    _return.ReturnType = ReturnTypeEnum.Error;
                    _return.Messages.Add(new() { ReturnType = ReturnTypeEnum.Error, Code = Codes._ERROR, Text = $"Ops, something went wrong by including the entity '{nameEntity}'!" });
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _return.ReturnType = ReturnTypeEnum.Error;
                _return.Messages.Add(new() { ReturnType = ReturnTypeEnum.Error, Code = Codes._ERROR, Text = ex.Message });

                bool possuiInnerException = ex.InnerException != null;
                var ex2 = ex.InnerException;

                while (possuiInnerException)
                {
                    _return.Messages.Add(new() { ReturnType = ReturnTypeEnum.Error, Code = Codes._ERROR, Text = ex2?.Message });
                    possuiInnerException = ex2?.InnerException != null;
                    ex2 = ex2?.InnerException;
                }

                logger.LogError($"ERROR (01): {_return.FormatMessage}");
            }
            catch (DbUpdateException ex)
            {
                _return.ReturnType = ReturnTypeEnum.Error;
                _return.Messages.Add(new() { ReturnType = ReturnTypeEnum.Error, Code = Codes._ERROR, Text = ex.Message });

                bool possuiInnerException = ex.InnerException != null;
                var ex2 = ex.InnerException;

                while (possuiInnerException)
                {
                    _return.Messages.Add(new() { ReturnType = ReturnTypeEnum.Error, Code = Codes._ERROR, Text = ex2?.Message });
                    possuiInnerException = ex2?.InnerException != null;
                    ex2 = ex2?.InnerException;
                }

                logger.LogError($"ERROR (02): {_return.FormatMessage}");
            }
            catch (Exception ex)
            {
                _return.ReturnType = ReturnTypeEnum.Error;
                _return.Messages.Add(new() { ReturnType = ReturnTypeEnum.Error, Code = Codes._ERROR, Text = ex.Message, Exception = ex });

                bool possuiInnerException = ex.InnerException != null;
                var ex2 = ex.InnerException;

                while (possuiInnerException)
                {
                    _return.Messages.Add(new() { ReturnType = ReturnTypeEnum.Error, Code = Codes._ERROR, Text = ex2?.Message, Exception = ex2 });
                    possuiInnerException = ex2?.InnerException != null;
                    ex2 = ex2?.InnerException;
                }

                logger.LogError($"ERROR (03): {_return.FormatMessage}");
            }

            return _return;
        }

        public virtual async Task<OperationReturn> UpdateAsync(IElement element)
        {
            OperationReturn _return = new() { ReturnType = ReturnTypeEnum.Success };

            if (element == null)
            {
                _return.ReturnType = ReturnTypeEnum.Error;
                _return.Messages.Add(new() { ReturnType = ReturnTypeEnum.Error, Code = Codes._ERROR, Text = "The entity is void!" });

                return _return;
            }

            _return = await EntityValidation(element);
            if (_return.Messages.Count > 0)
            {
                logger.LogWarning($"UpdateAsync: {element.GetType().Name} - {GetKeyValue(element)} - {_return.FormatMessage}");
                return _return;
            }

            var nameEntity = GetDisplayName(element);

            _return.EntityName = element.GetType().Name;
            _return.Key = GetKeyValue(element);
            _return.Field = GetKeyName(element);

            try
            {
                EntityHierarchy(element);

                persistenceContext.Attach(element);
                persistenceContext.Entry(element).State = EntityState.Modified;

                logger.LogInformation($"UpdateAsync: {element.GetType().Name} - {GetKeyValue(element)} - {nameEntity} - SaveChangesAsync");
                var returnSave = await persistenceContext.SaveChangesAsync();

                if (returnSave > 0)
                {
                    logger.LogInformation($"'{nameEntity}' successfully added!");
                    _return.ReturnType = ReturnTypeEnum.Success;
                    _return.Messages.Add(new() { ReturnType = ReturnTypeEnum.Success, Code = Codes._SUCCESS, Text = $"'{nameEntity}' updated successfully!" });
                }
                else
                {
                    logger.LogWarning($"Ops, something went wrong by including the entity '{nameEntity}'!");
                    _return.Messages.Add(new() { ReturnType = ReturnTypeEnum.Error, Code = Codes._ERROR, Text = $"Ops, something went wrong updating the entity '{nameEntity}'!" });
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _return.ReturnType = ReturnTypeEnum.Error;
                _return.Messages.Add(new() { ReturnType = ReturnTypeEnum.Error, Code = Codes._ERROR, Text = ex.Message });

                bool possuiInnerException = ex.InnerException != null;
                var ex2 = ex.InnerException;

                while (possuiInnerException)
                {
                    _return.Messages.Add(new() { ReturnType = ReturnTypeEnum.Error, Code = Codes._ERROR, Text = ex2?.Message });
                    possuiInnerException = ex2?.InnerException != null;
                    ex2 = ex2?.InnerException;
                }

                logger.LogError($"ERROR (01): {_return.FormatMessage}");
            }
            catch (DbUpdateException ex)
            {
                _return.ReturnType = ReturnTypeEnum.Error;
                _return.Messages.Add(new() { ReturnType = ReturnTypeEnum.Error, Code = Codes._ERROR, Text = ex.Message });

                bool possuiInnerException = ex.InnerException != null;
                var ex2 = ex.InnerException;

                while (possuiInnerException)
                {
                    _return.Messages.Add(new() { ReturnType = ReturnTypeEnum.Error, Code = Codes._ERROR, Text = ex2?.Message });
                    possuiInnerException = ex2?.InnerException != null;
                    ex2 = ex2?.InnerException;
                }

                logger.LogError($"ERROR (02): {_return.FormatMessage}");
            }
            catch (Exception ex)
            {
                _return.ReturnType = ReturnTypeEnum.Error;
                _return.Messages.Add(new() { ReturnType = ReturnTypeEnum.Error, Code = Codes._ERROR, Text = ex.Message, Exception = ex });

                bool possuiInnerException = ex.InnerException != null;
                var ex2 = ex.InnerException;

                while (possuiInnerException)
                {
                    _return.Messages.Add(new() { ReturnType = ReturnTypeEnum.Error, Code = Codes._ERROR, Text = ex2?.Message, Exception = ex2 });
                    possuiInnerException = ex2?.InnerException != null;
                    ex2 = ex2?.InnerException;
                }

                logger.LogError($"ERROR (03): {_return.FormatMessage}");
            }

            return _return;
        }

        public virtual async Task<OperationReturn> DeleteAsync(IElement element)
        {
            OperationReturn _return = new() { ReturnType = ReturnTypeEnum.Success };

            if (element == null)
            {
                _return.ReturnType = ReturnTypeEnum.Error;
                _return.Messages.Add(new() { ReturnType = ReturnTypeEnum.Error, Code = Codes._ERROR, Text = "The entity is void!" });

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
                    _return.Messages.Add(new() { ReturnType = ReturnTypeEnum.Success, Code = Codes._SUCCESS, Text = $"'{nameEntity}' successfully deleted!" });
                }
                else
                {
                    _return.ReturnType = ReturnTypeEnum.Error;
                    _return.Messages.Add(new() { ReturnType = ReturnTypeEnum.Error, Code = Codes._ERROR, Text = $"Ops, something went wrong deleting the entity '{nameEntity}'!" });
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _return.ReturnType = ReturnTypeEnum.Error;
                _return.Messages.Add(new() { ReturnType = ReturnTypeEnum.Error, Code = Codes._ERROR, Text = ex.Message });

                bool possuiInnerException = ex.InnerException != null;
                var ex2 = ex?.InnerException;

                while (possuiInnerException)
                {
                    _return.Messages.Add(new() { ReturnType = ReturnTypeEnum.Error, Code = Codes._ERROR, Text = ex2?.Message });
                    possuiInnerException = ex2?.InnerException != null;
                    ex2 = ex2?.InnerException;
                }
                logger.LogError($"ERROR (01): {_return.FormatMessage}");
            }
            catch (DbUpdateException ex)
            {
                _return.ReturnType = ReturnTypeEnum.Error;
                _return.Messages.Add(new() { ReturnType = ReturnTypeEnum.Error, Code = Codes._ERROR, Text = ex.Message });

                bool possuiInnerException = ex.InnerException != null;
                var ex2 = ex.InnerException;

                while (possuiInnerException)
                {
                    _return.Messages.Add(new() { ReturnType = ReturnTypeEnum.Error, Code = Codes._ERROR, Text = ex2?.Message });
                    possuiInnerException = ex2?.InnerException != null;
                    ex2 = ex2?.InnerException;
                }
                logger.LogError($"ERROR (02): {_return.FormatMessage}");
            }
            catch (Exception ex)
            {
                _return.ReturnType = ReturnTypeEnum.Error;
                _return.Messages.Add(new()
                {
                    ReturnType = ReturnTypeEnum.Error,
                    Code = Codes._ERROR,
                    Text = $"An unexpected error occurred in the deletion of the entity '{nameEntity}'."
                });
                _return.Messages.Add(new() { ReturnType = ReturnTypeEnum.Error, Code = Codes._ERROR, Text = ex.Message });

                bool possuiInnerException = ex.InnerException != null;
                var ex2 = ex?.InnerException;

                while (possuiInnerException)
                {
                    _return.Messages.Add(new() { ReturnType = ReturnTypeEnum.Error, Code = Codes._ERROR, Text = ex2?.Message });
                    possuiInnerException = ex2?.InnerException != null;
                    ex2 = ex2?.InnerException;
                }
                logger.LogError($"ERROR (03): {_return.FormatMessage}");
            }

            return _return;
        }

        public virtual async Task<OperationReturn> UpdateList(IElement mainElement)
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
                logger.LogInformation($"Checks if property '{property}' it's a first or foreign key!");
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
                            var elementsDelete = propertyElement.PropertyInfo.GetValue(entityProperty.GetType(), null) as ICollection<IElement>;
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

                var elements = property!.PropertyInfo!.GetValue(mainElement, null) as ICollection<IElement>;

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

        public virtual void EntityHierarchy(IElement element) { }

        public virtual async Task<IEnumerable<IElement>> Filter(Expression<Func<IElement, bool>> filter
           , params Expression<Func<IElement, object>>[] includes)
        {
            var query = persistenceContext.Set<IElement>()
                    .AsNoTrackingWithIdentityResolution();

            if (includes is not null && includes.Length > 0)
            {
                foreach (var include in includes)
                    query = query.Include(include);
            }

            return await query.Where(filter)
                    .ToListAsync();
        }

        public virtual async Task<IEnumerable<IElement>> Filter(Expression<Func<IElement, bool>> filter)
        {
            return await persistenceContext.Set<IElement>()
                    .AsNoTrackingWithIdentityResolution()
                    .Where(filter)
                    .ToListAsync();
        }

        public virtual async Task<IEnumerable<IElement>> Filter(Expression<Func<IElement, bool>> filter
            , int pageNumber, int pageSize, params Expression<Func<IElement, object>>[] includes)
        {
            var query = persistenceContext.Set<IElement>()
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

        public virtual async Task<IEnumerable<IElement>> Filter(Expression<Func<IElement, bool>> filter
            , int pageNumber, int pageSize)
        {
            return await persistenceContext.Set<IElement>()
                    .AsNoTrackingWithIdentityResolution()
                    .Where(filter)
                    .Skip(pageSize * (pageNumber - 1))
                    .Take(pageSize)
                    .ToListAsync();
        }

        public virtual async Task<IEnumerable<IElement>> Paginate(int pageNumber, int pageSize
            , params Expression<Func<IElement, object>>[] includes)
        {
            var query = persistenceContext.Set<IElement>()
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

        public virtual async Task<IEnumerable<IElement>> Paginate(int pageNumber, int pageSize)
        {
            var query = persistenceContext.Set<IElement>()
                    .AsNoTrackingWithIdentityResolution();

            return await query.Skip(pageSize * (pageNumber - 1))
                    .Take(pageSize)
                    .ToListAsync();
        }

        public virtual async Task<IElement> GetEntityTrackingByIdAsync(long id)
        {
#pragma warning disable CS8603 // Possível retorno de referência nula.
            return await persistenceContext.Set<IElement>()
                    .AsTracking()
                    .FirstOrDefaultAsync(find => find.Id == id);
#pragma warning restore CS8603 // Possível retorno de referência nula.
        }

        public virtual async Task<IElement> GetEntityByIdAsync(long id)
        {
#pragma warning disable CS8603 // Possível retorno de referência nula.
            return await persistenceContext.Set<IElement>()
                    .AsNoTrackingWithIdentityResolution()
                    .FirstOrDefaultAsync(find => find.Id == id);
#pragma warning restore CS8603 // Possível retorno de referência nula.
        }

        public virtual async Task<IElement> GetEntityByIdAsync(long id, params Expression<Func<IElement, object>>[] includes)
        {
            var query = persistenceContext.Set<IElement>()
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
        private async Task<OperationReturn> EntityValidation(IElement element)
        {
            var operationReturn = new OperationReturn { ReturnType = ReturnTypeEnum.Warning };

            if (element is null)
            {
                operationReturn.Key = "0";
                operationReturn.EntityName = "[unknown]";
                operationReturn.ReturnType = ReturnTypeEnum.Error;
                operationReturn.Messages.Add(new()
                {
                    ReturnType = operationReturn.ReturnType,
                    Code = Codes._ERROR,
                    Text = "Entity cannot be null!"
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

                        var elementBase = propertyForeignKey.GetValue(element, null) as IElement;

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
                                    Code = Codes._WARNING,
                                    Text = message
                                });
                            }
                            else
                            {
                                operationReturn.Messages.Add(new()
                                {
                                    ReturnType = ReturnTypeEnum.Warning,
                                    Code = Codes._WARNING,
                                    Text = $"'{property.Name}' required, but no message configured on the entity!"
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
                                    operationReturn.Messages.Add(new() { ReturnType = ReturnTypeEnum.Warning, Code = Codes._WARNING, Text = message });
                                }
                            }
                            else
                            {
                                operationReturn.Messages.Add(new()
                                {
                                    ReturnType = ReturnTypeEnum.Warning,
                                    Code = Codes._WARNING,
                                    Text = $"'{property.Name}' required, but no message configured on the entity!"
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
                                    operationReturn.Messages.Add(new() { ReturnType = ReturnTypeEnum.Warning, Code = Codes._WARNING, Text = message });
                                }
                            }
                            else
                            {
                                operationReturn.Messages.Add(new()
                                {
                                    ReturnType = ReturnTypeEnum.Warning,
                                    Code = Codes._WARNING,
                                    Text = $"'{property.Name}' required, but no message configured on the entity!"
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
                                    operationReturn.Messages.Add(new() { ReturnType = ReturnTypeEnum.Warning, Code = Codes._WARNING, Text = message });
                                }
                            }
                            else
                            {
                                operationReturn.Messages.Add(new()
                                {
                                    ReturnType = ReturnTypeEnum.Warning,
                                    Code = Codes._WARNING,
                                    Text = $"'{property.Name}' required, but no message configured on the entity!"
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
                                    operationReturn.Messages.Add(new() { ReturnType = ReturnTypeEnum.Warning, Code = Codes._WARNING, Text = message });
                                }
                            }
                            else
                            {
                                operationReturn.Messages.Add(new()
                                {
                                    ReturnType = ReturnTypeEnum.Warning,
                                    Code = Codes._WARNING,
                                    Text = $"'{property.Name}' required, but no message configured on the entity!"
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
                                    operationReturn.Messages.Add(new() { ReturnType = ReturnTypeEnum.Warning, Code = Codes._WARNING, Text = message });
                                }
                            }
                            else
                            {
                                operationReturn.Messages.Add(new()
                                {
                                    ReturnType = ReturnTypeEnum.Warning,
                                    Code = Codes._WARNING,
                                    Text = $"'{property.Name}' required, but no message configured on the entity!"
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
                                    operationReturn.Messages.Add(new() { ReturnType = ReturnTypeEnum.Warning, Code = Codes._WARNING, Text = message });
                                }
                            }
                            else
                            {
                                operationReturn.Messages.Add(new()
                                {
                                    ReturnType = ReturnTypeEnum.Warning,
                                    Code = Codes._WARNING,
                                    Text = $"'{property.Name}' required, but no message configured on the entity!"
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
                                    operationReturn.Messages.Add(new() { ReturnType = ReturnTypeEnum.Warning, Code = Codes._WARNING, Text = message });
                                }
                            }
                            else
                            {
                                operationReturn.Messages.Add(new()
                                {
                                    ReturnType = ReturnTypeEnum.Warning,
                                    Code = Codes._WARNING,
                                    Text = $"'{property.Name}' required, but no message configured on the entity!"
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
                                    operationReturn.Messages.Add(new() { ReturnType = ReturnTypeEnum.Warning, Code = Codes._WARNING, Text = message });
                                }
                            }
                            else
                            {
                                operationReturn.Messages.Add(new()
                                {
                                    ReturnType = ReturnTypeEnum.Warning,
                                    Code = Codes._WARNING,
                                    Text = $"'{property.Name}' required, but no message configured on the entity!"
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
                                    operationReturn.Messages.Add(new() { ReturnType = ReturnTypeEnum.Warning, Code = Codes._WARNING, Text = message });
                                }

                                var argument2 = property.PropertyInfo.CustomAttributes.ElementAt(1).NamedArguments.ElementAt(0);
                                if (argument2.MemberName == Codes._ERRORMESSAGE)
                                {
                                    var message = argument2.TypedValue.Value?.ToString();
                                    operationReturn.Messages.Add(new() { ReturnType = ReturnTypeEnum.Warning, Code = Codes._WARNING, Text = message });
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
                    Code = Codes._WARNING,
                    Text = $"Entity '{element.GetType()}' not found!"
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

        private string GetKeyValue(IElement element)
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

        private string GetKeyName(IElement element)
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

        private string GetDisplayName(IElement element)
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