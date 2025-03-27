using AutoMapper;
using PersistenceNet.AppServices;
using PersistenceNet.Interfaces;
using PersistenceNet.Test.Domain.Entitys;
using PersistenceNet.Test.Domain.Services;
using PersistenceNet.Test.Domain.ViewModels;

namespace PersistenceNet.Test.Domain.AppServices
{
    public class ClassificationAppService(IClassificationService classificationService
                                        , ITransactionWork transactionWork
                                        , IMapper mapper
                                        , ILogger<ClassificationViewModel> logger)
        : AppServiceBase<ClassificationViewModel, Classification>(classificationService, transactionWork, mapper, logger), IClassificationAppService
    {  }
}