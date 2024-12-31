using Microsoft.EntityFrameworkCore;
using PersistenceNet.Enuns;
using PersistenceNet.Test.Domain.Entitys;

namespace PersistenceNet.Test.Domain
{
    public class ContextTest(DbContextOptions<ContextTest> options)
        : PersistenceContext(options)
    {
        public DbSet<Classification> Classifications { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Classification>(cla => cla.Property(cla => cla.Active)
                .HasConversion(cla => (int)cla, cla => (ActiveEnum)cla));

            base.OnModelCreating(modelBuilder);
        }
    }
}