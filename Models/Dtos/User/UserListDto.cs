using Models.Enums;

namespace Models.Dtos.User;

public record UserListDto
{
    public int Id { get; init; }

    public string Username { get; init; } = null!;

    public string Firstname { get; init; } = null!;

    public string Lastname { get; init; } = null!;

    public string Email { get; init; } = null!;

    public DateTime Birthdate { get; init; }

    public string? PhoneNumber { get; init; }

    public GenderType Gender { get; init; }

    public int TeamId { get; init; }

    public bool IsActive { get; init; }
}
