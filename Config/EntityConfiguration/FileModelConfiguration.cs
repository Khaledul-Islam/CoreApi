using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Entities.Files;

namespace Config.EntityConfiguration
{
    public class FileModelConfiguration : IEntityTypeConfiguration<FileModel>
    {
        public void Configure(EntityTypeBuilder<FileModel> builder)
        {
            builder.Property(p => p.Name).IsRequired().HasMaxLength(200);

            builder.Property(p => p.FileType).IsRequired().HasMaxLength(200);

            builder.Property(p => p.Extension).IsRequired().HasMaxLength(10);
            builder.Property(p => p.FilePath).HasMaxLength(500);

            builder.Property(p => p.Description).HasMaxLength(200);
        }
    }
}
