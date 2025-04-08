using PersistenceNet.Test.Domain.Entitys;
using PersistenceNet.Repositorys;
using PersistenceNet.Interfaces;

namespace PersistenceNet.Test.Domain.Repositorys
{
    public class ClassificationRepository(ContextTest contextTest
                                        , ILogger<PersistenceData<ContextTest, Classification>> logger
                                        , IMessagesProvider provider)
        : PersistenceData<ContextTest, Classification>(contextTest, logger, provider), IClassificationRepository { }
}