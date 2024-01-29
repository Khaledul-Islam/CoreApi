using Config.EntityConfiguration;
using Microsoft.EntityFrameworkCore;

namespace Data.Context
{
    public class ApplicationDbContext(DbContextOptions options) : DbContext(options)
    {
        //public DbSet<User> Users { get; set; }
        // public DbSet<Role> Roles { get; set; }
        //public DbSet<UserRole> UserRoles { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new RoleConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
            modelBuilder.ApplyConfiguration(new FileModelConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
