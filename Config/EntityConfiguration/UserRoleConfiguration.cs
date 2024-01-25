using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Entities.Identity;

namespace Config.EntityConfiguration
{
    public class UserRoleConfiguration : IEntityTypeConfiguration<UserRole>
    {
        public void Configure(EntityTypeBuilder<UserRole> builder)
        {

            builder.HasKey(a => a.Id);
            builder.ToTable(nameof(UserRole));
            builder.HasOne(p => p.User)
                .WithMany(p => p.UserRoles)
                .HasForeignKey(p => p.UserId);

            builder.HasOne(p => p.Role)
                .WithMany(p => p.UserRoles)
                .HasForeignKey(p => p.RoleId);
        }
    }

}
