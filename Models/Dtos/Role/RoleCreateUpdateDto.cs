namespace Models.Dtos.Role;

public record RoleCreateUpdateDto
{
    public string Name { get; init; } = null!;

    public string Description { get; init; } = null!;
}

