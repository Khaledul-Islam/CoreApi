namespace Models.Entities.Identity;

public class UserRole
{
    public int Id { get; set; }
    public int UserId { get; set; }
    public int RoleId { get; set; }
    public DateTime CreatedOn { get; set; }
    public User User { get; set; } = null!;

    public Role Role { get; set; } = null!;
}

