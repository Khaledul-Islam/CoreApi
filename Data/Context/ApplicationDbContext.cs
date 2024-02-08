using Config.EntityConfiguration.Example;
using Config.EntityConfiguration;
using Microsoft.EntityFrameworkCore;

namespace Data.Context
{
    public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
    {
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
            modelBuilder.ApplyConfiguration(new FileModelConfiguration());
            modelBuilder.ApplyConfiguration(new TestConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}
