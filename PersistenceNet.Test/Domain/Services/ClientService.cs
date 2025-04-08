using PersistenceNet.Interfaces;
using PersistenceNet.Services;
using PersistenceNet.Test.Domain.Entitys;
using PersistenceNet.Test.Domain.Repositorys;

namespace PersistenceNet.Test.Domain.Services
{
    public class ClientService(IClientRepository repository, IMessagesProvider provider)
        : ServiceBase<Client>(repository, provider), IClientService
    { }
}