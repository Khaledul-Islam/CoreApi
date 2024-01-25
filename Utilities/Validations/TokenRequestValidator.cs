using FluentValidation;
using Models.Dtos.AuthToken;

namespace Utilities.Validations
{
    public class TokenRequestValidator : BaseValidator<TokenRequest>
    {
        public TokenRequestValidator()
        {
            List<string> validGrantTypes = new() { "password", "refresh_token" };

            RuleFor(p => p.GrantType)
                .NotEmpty().MaximumLength(15)
                .Must(p => p is not null && validGrantTypes.Contains(p.ToLower()))
                .WithMessage("Not valid grantType");

            RuleFor(p => p.Username).NotEmpty().MaximumLength(40);

            RuleFor(p => p.Password).NotEmpty().MinimumLength(6).MaximumLength(127);

            RuleFor(p => p.RefreshToken).MaximumLength(50);
        }
    }

}
