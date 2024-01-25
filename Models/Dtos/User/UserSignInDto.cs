using Models.Dtos.AuthToken;

namespace Models.Dtos.User;

public record UserSignInDto
{
    public UserDto UserDto { get; init; } = null!;

    public Token Token { get; init; } = null!;
}
