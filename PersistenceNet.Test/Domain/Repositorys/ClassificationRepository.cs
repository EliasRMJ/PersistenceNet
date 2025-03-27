using PersistenceNet.Test.Domain.Entitys;
using PersistenceNet.Repositorys;

namespace PersistenceNet.Test.Domain.Repositorys
{
    public class ClassificationRepository(ContextTest contextTest, ILogger<PersistenceData<ContextTest, Classification>> logger)
        : PersistenceData<ContextTest, Classification>(contextTest, logger), IClassificationRepository
    { }
}