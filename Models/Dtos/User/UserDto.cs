﻿using Models.Enums;

namespace Models.Dtos.User;

public class UserDto
{
    public int Id { get; set; }

    public string Username { get; set; } = null!;

    public string Firstname { get; set; } = null!;

    public string Lastname { get; set; } = null!;

    public string Email { get; set; } = null!;

    public DateTime Birthdate { get; set; }

    public string? PhoneNumber { get; set; }

    public GenderType Gender { get; set; }

    public ICollection<string> Roles { get; set; } = null!;

    public bool IsActive { get; set; }
}
