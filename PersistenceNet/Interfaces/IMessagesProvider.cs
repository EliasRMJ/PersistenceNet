using PersistenceNet.MessagesProvider;

namespace PersistenceNet.Interfaces
{
    public interface IMessagesProvider
    {
        Messages Current { get; }
    }
}