using PersistenceNet.Transactions;

namespace PersistenceNet.Test.Domain.Transaction
{
    public class TransactionWork(ContextTest context)
        : TransactionWorkBase(context)
    { }
}