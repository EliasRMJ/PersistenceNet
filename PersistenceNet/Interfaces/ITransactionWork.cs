using System.Transactions;

namespace PersistenceNet.Interfaces
{
    public interface ITransactionWork
    {
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
}