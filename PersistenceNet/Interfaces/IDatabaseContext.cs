namespace PersistenceNet.Interfaces
{
    public interface IDatabaseContext
    {
        Task<int> SaveChangesAsync();
    }
}