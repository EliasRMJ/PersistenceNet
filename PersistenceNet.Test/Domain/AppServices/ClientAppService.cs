using AutoMapper;
using PersistenceNet.AppServices;
using PersistenceNet.Constants;
using PersistenceNet.Enuns;
using PersistenceNet.Interfaces;
using PersistenceNet.Structs;
using PersistenceNet.Test.Domain.Entitys;
using PersistenceNet.Test.Domain.Services;
using PersistenceNet.Test.Domain.ViewModels;
using PersistenceNet.Utils;
using System.Linq.Expressions;

namespace PersistenceNet.Test.Domain.AppServices
{
    public class ClientAppService(IClientService clientService
                                , ITransactionWork transactionWork
                                , IMapper mapper
                                , ILogger<ClientViewModel> logger)
        : AppServiceBase<ClientViewModel, Client>(clientService, transactionWork, mapper, logger), IClientAppService
    {
        public override async Task<OperationReturn> UpdateAsync(ClientViewModel element)
        {
            var clientExist = await clientService.GetEntityByIdAsync(element.ClientId);
            if (clientExist is null)
            {
                var operationResult = new OperationReturn
                {
                    EntityName = "Client",
                    Field = "ClientId",
                    Key = element.ClientId.ToString(),
                    ReturnType = ReturnTypeEnum.Empty,
                    Messages = [ new() { Code = Codes._WARNING, Text = $"Client {element.ClientId} not found!" } ]
                };
                return operationResult;
            }

            return await base.UpdateAsync(element);
        }

        public override async Task<IEnumerable<ClientViewModel>> Filter(Expression<Func<ClientViewModel, bool>> filter
            , int pageNumber, int pageSize)
        {
            logger.LogInformation("Starting the filter method in 'ClientAppService'.");
            var filterConvert = ExpressionFuncConvert.Builder<ClientViewModel, Client>(filter, "Person");
            var resultList = await clientService.Filter(filterConvert, pageNumber, pageSize
                                                      , inc => inc.Person
                                                      , inc => inc.Classification
                                                      , inc => inc.Person.Emails);

            if (resultList is null || !resultList.Any())
            {
                logger.LogWarning("No records found at this time with the filters provided.");
                throw new Exception("No records found at this time with the filters provided.");
            }

            logger.LogInformation("Converting the entity to the object successfully performed in the filter method.");
            return mapper.Map<IEnumerable<ClientViewModel>>(resultList);
        }
    }
}