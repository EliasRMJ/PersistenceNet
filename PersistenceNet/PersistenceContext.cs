using Microsoft.EntityFrameworkCore;

namespace PersistenceNet
{
    public class PersistenceContext(DbContextOptions<PersistenceContext> options)
        : DbContext(options)
    {
        public bool IsConnect { get { return base.Database.CanConnect(); } }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }
    }
}