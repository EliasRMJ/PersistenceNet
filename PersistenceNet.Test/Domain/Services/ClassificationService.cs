using PersistenceNet.Services;
using PersistenceNet.Test.Domain.Entitys;
using PersistenceNet.Test.Domain.Repositorys;

namespace PersistenceNet.Test.Domain.Services
{
    public class ClassificationService(IClassificationRepository repository)
        : ServiceBase<Classification>(repository), IClassificationService
    { }
}