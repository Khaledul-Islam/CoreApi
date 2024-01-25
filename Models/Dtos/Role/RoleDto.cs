namespace Models.Dtos.Role;

public record RoleDto
{
    public int Id { get; init; }

    public string Name { get; init; } = null!;

    public string Description { get; init; } = null!;
}
