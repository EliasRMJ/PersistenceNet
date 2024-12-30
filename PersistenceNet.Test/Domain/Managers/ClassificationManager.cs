using PersistenceNet.Extensions;
using PersistenceNet.Structs;
using PersistenceNet.Test.Domain.Entitys;
using PersistenceNet.Test.Domain.Repositorys;
using PersistenceNet.Test.Domain.Views;

namespace PersistenceNet.Test.Domain.Managers
{
    public sealed class ClassificationManager : Manager, IClassificationManager
    {
        private readonly IClassificationRepository _classificationRepository;

        public ClassificationManager(ContextTest contextTest)
        {
            base._contextTest = contextTest;
            _classificationRepository = new ClassificationRepository(contextTest);
        }

        public async Task<OperationReturn> CreateOrReplace(ClassificationView classification)
        {
            var classificationConvert = classification.ConvertTo(classification) as Classification;
            return classification.IsNew ?
                await _classificationRepository.New(classificationConvert!) :
                await _classificationRepository.Update(classificationConvert!);
        }

        public async Task<ClassificationView?> Get(int id)
        {
            var classification = await _classificationRepository.Get(id);
            return classification?.GetView() as ClassificationView;
        }

        public async Task<IEnumerable<ClassificationView>> List()
        {
            var classifications = await _classificationRepository.List();
            return classifications.ConvertElementToView().Cast<ClassificationView>();
        }
    }
}