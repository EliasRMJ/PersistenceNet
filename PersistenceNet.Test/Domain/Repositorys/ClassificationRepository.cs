using PersistenceNet.Test.Domain.Entitys;
using Microsoft.EntityFrameworkCore;
using PersistenceNet.Structs;

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
                   .Where(c => c.Active == "S")
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
    }
}