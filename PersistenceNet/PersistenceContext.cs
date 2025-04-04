using Flunt.Notifications;
using Microsoft.EntityFrameworkCore;
using PersistenceNet.Interfaces;

namespace PersistenceNet
{
    public class PersistenceContext(DbContextOptions options)
        : DbContext(options), IDatabaseContext
    {
        public bool IsCanConnect { get { return base.Database.CanConnect(); } }

        public async Task<int> SaveChangesAsync()
        {
            return await base.SaveChangesAsync();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.EnableSensitiveDataLogging();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Ignore<Notification>();

            base.OnModelCreating(modelBuilder);
        }
    }
}