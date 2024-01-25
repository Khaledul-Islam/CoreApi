using FluentValidation;
using Models.Dtos.User;

namespace Utilities.Validations
{
    public class UserUpdateDtoValidator : BaseValidator<UserUpdateDto>
    {
        public UserUpdateDtoValidator()
        {
            RuleFor(p => p.Username).NotEmpty().MaximumLength(40);

            RuleFor(p => p.Firstname).NotEmpty().MaximumLength(35);

            RuleFor(p => p.Lastname).NotEmpty().MaximumLength(35);

            RuleFor(p => p.Email).NotEmpty().EmailAddress().MaximumLength(320);

            RuleFor(p => p.PhoneNumber).MaximumLength(15).Matches(@"\(?\d{3}\)?-? *\d{3}-? *-?\d{4}");

            RuleFor(p => p.Gender).IsInEnum();

            When(p => p.TeamId.HasValue, () => RuleFor(p => p.TeamId).GreaterThanOrEqualTo(1));
        }
    }
}
