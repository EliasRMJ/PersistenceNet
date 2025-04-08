using PersistenceNet.Interfaces;
using PersistenceNet.Services;
using PersistenceNet.Test.Domain.Entitys;
using PersistenceNet.Test.Domain.Repositorys;

namespace PersistenceNet.Test.Domain.Services
{
    public class ClassificationService(IClassificationRepository repository, IMessagesProvider provider)
        : ServiceBase<Classification>(repository, provider), IClassificationService
    { }
}