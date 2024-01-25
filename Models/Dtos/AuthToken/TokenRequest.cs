namespace Models.Dtos.AuthToken;

public record TokenRequest
{
    public string GrantType { get; init; } = null!;

    public string Username { get; init; } = null!;

    public string Password { get; init; } = null!;

    public string? RefreshToken { get; init; }

    public string? AccessToken { get; init; }
}

