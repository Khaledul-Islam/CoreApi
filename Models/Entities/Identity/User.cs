using System.ComponentModel.DataAnnotations;
using Models.Enums;

namespace Models.Entities.Identity;

public class User
{
    [Key]
    public int Id { get; set; }
    public string Firstname { get; set; } = null!;
    public string Password { get; set; } = null!;
    public string Lastname { get; set; } = null!;
    public string UserName { get; set; } = null!;

    public DateTime Birthdate { get; set; }

    public GenderType Gender { get; set; }

    public bool IsActive { get; set; } = true;

    public DateTime? LastLoginDate { get; set; }

    public string? RefreshToken { get; set; }
    public string? Email { get; set; }

    public DateTime? RefreshTokenExpirationTime { get; set; }

    public DateTime CreatedOn { get; set; }

    // Navigation properties
    //public Team Team { get; set; } = null!;

    // public FileModel? ProfilePicture { get; set; }

    public ICollection<UserRole>? UserRoles { get; set; } 
    //public ICollection<EmailsLog>? EmailsLogs { get; set; }
}