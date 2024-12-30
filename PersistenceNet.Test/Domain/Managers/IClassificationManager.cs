using PersistenceNet.Structs;
using PersistenceNet.Test.Domain.Views;

namespace PersistenceNet.Test.Domain.Managers
{
    public interface IClassificationManager
    {
        Task<OperationReturn> CreateOrReplace(ClassificationView classification);
        Task<ClassificationView?> Get(int id);
        Task<IEnumerable<ClassificationView>> List();
    }
}