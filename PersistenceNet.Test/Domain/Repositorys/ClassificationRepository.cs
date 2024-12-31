using PersistenceNet.Test.Domain.Entitys;
using Microsoft.EntityFrameworkCore;
using PersistenceNet.Structs;
using PersistenceNet.Enuns;

namespace PersistenceNet.Test.Domain.Repositorys
{
    internal class ClassificationRepository(ContextTest contextTest) 
        : PersistenceData<Classification>(contextTest), IClassificationRepository
    {
        private readonly ContextTest _contextTest = contextTest;

        async public Task<Classification?> Get(int id)
        {
            return await this._contextTest.Classifications
                   .AsNoTrackingWithIdentityResolution()
                   .FirstOrDefaultAsync(c => c.Id == id);
        }

        async public Task<List<Classification>> List()
        {
            return await this._contextTest.Classifications
                   .AsNoTrackingWithIdentityResolution()
                   .Where(c => c.Active == ActiveEnum.S)
                   .ToListAsync();
        }

        public Task<OperationReturn> New(Classification classification)
        {
            return NewAsync(classification);
        }

        public Task<OperationReturn> Update(Classification classification)
        {
            return UpdateAsync(classification);
        }

        /* Method used only when there is a need to write inherited understood.
        protected override void EntityHierarchy(IElement element)
        {
            if (element.ElementStates == ElementStatesEnum.New)
                this._contextTest.Classifications.Add((Classification)element);
            else
                this._contextTest.Classifications.UpdateRange((Classification)element);
        }
        */
    }
}