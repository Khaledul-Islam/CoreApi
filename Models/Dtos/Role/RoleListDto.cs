﻿namespace Models.Dtos.Role;

public record RoleListDto
{
    public int Id { get; init; }

    public string Name { get; init; } = null!;

    public string Description { get; init; } = null!;

    public string NormalizedName { get; init; } = null!;
}
