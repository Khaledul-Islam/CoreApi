﻿using Models.Enums;

namespace Models.Dtos.User;

public record CurrentUser
{
    public int Id { get; init; }

    public string Username { get; init; } = null!;

    public string Firstname { get; init; } = null!;

    public string Lastname { get; init; } = null!;

    public string Email { get; init; } = null!;

    public DateTime Birthdate { get; init; }

    public string PhoneNumber { get; init; } = null!;

    public GenderType Gender { get; init; }

    public ICollection<string> Roles { get; init; } = null!;


}
