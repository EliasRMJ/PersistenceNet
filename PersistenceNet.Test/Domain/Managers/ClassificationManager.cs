using PersistenceNet.Enuns;
using PersistenceNet.Structs;
using PersistenceNet.Test.Domain.Entitys;
using PersistenceNet.Test.Domain.Repositorys;
using PersistenceNet.Test.Domain.Views;

namespace PersistenceNet.Test.Domain.Managers
{
    public sealed class ClassificationManager : Manager, IClassificationManager
    {
        readonly IClassificationRepository _classificationRepository;

        public ClassificationManager(ContextTest contextTest)
        {
            base._contextTest = contextTest;
            _classificationRepository = new ClassificationRepository(contextTest);
        }

        public async Task<OperationReturn> CreateOrReplace(ClassificationView classification)
        {
            _ = new OperationReturn { EntityName = "Classification", ReturnType = ReturnTypeEnum.Warning };
            var objResult = await _classificationRepository.Get(classification.Id);
            OperationReturn operationReturn;
            if (objResult == null)
            {
                objResult = new Classification
                {
                    Name = classification.Name,
                    Active = classification.IsActive ? "S" : "N"
                };

                operationReturn = await _classificationRepository.New(objResult);
            }
            else
            {
                objResult.Name = classification.Name;
                objResult.Active = classification.IsActive ? "S" : "N";

                operationReturn = await _classificationRepository.Update(objResult);
            }

            return operationReturn;
        }

        public async Task<ClassificationView> Get(int id)
        {
            var objResult = await _classificationRepository.Get(id);
            var classification = new ClassificationView();
            if (objResult != null)
            {
                classification.Id = objResult.Id;
                classification.Name = objResult.Name;
                classification.IsActive = (objResult.Active == "S");
            }

            return classification;
        }

        public async Task<ClassificationView[]> List()
        {
            var listResult = await _classificationRepository.List();
            var count = listResult.Count;
            var classifications = new ClassificationView[count];
            
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    classifications[i].Id = listResult[i].Id;
                    classifications[i].Name = listResult[i].Name;
                    classifications[i].IsActive = (listResult[i].Active == "S");
                }  
            }

            return classifications;
        }
    }
}