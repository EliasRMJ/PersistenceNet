using Microsoft.EntityFrameworkCore;
using PersistenceNet.Test.Domain.Entitys;

namespace PersistenceNet.Test.Domain
{
    public class ContextTest(DbContextOptions<ContextTest> options)
        : PersistenceContext(options)
    {
        public DbSet<Classification> Classifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}