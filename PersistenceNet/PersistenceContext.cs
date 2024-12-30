using Microsoft.EntityFrameworkCore;

namespace PersistenceNet
{
    public class PersistenceContext(DbContextOptions options)
        : DbContext(options)
    {
        public bool IsCanConnect { get { return base.Database.CanConnect(); } }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }
    }
}