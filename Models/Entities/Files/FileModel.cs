using Models.Entities.Identity;

namespace Models.Entities.Files;

public class FileModel
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public string FileType { get; set; } = null!;

    public string Extension { get; set; } = null!;

    public string? Description { get; set; }
    public byte[]? Data { get; set; } = null!;
    public string? FilePath { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

}

