using PersistenceNet.Structs;
using PersistenceNet.Test.Domain.Entitys;

namespace PersistenceNet.Test.Domain.Repositorys
{
    internal interface IClassificationRepository
    {
        Task<OperationReturn> New(Classification classification);
        Task<OperationReturn> Update(Classification classification);
        Task<Classification?> Get(int id);
        Task<List<Classification>> List();
    }
}