using PersistenceNet.Interfaces;
using PersistenceNet.Transactions;

namespace PersistenceNet.Test.Domain.Transaction
{
    public class TransactionWork(ContextTest context, IMessagesProvider provider)
        : TransactionWorkBase(context, provider) { }
}