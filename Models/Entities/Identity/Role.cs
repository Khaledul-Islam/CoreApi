namespace Models.Entities.Identity;

public class Role
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string Description { get; set; } = null!;

    public DateTime CreatedOn { get; set; }

    // Navigation Properties
    public ICollection<UserRole>? UserRoles { get; set; }
}


