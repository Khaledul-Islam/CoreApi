using FluentValidation;
using Models.Dtos.Role;

namespace Utilities.Validations
{
    public class RoleCreateUpdateDtoValidator : BaseValidator<RoleCreateUpdateDto>
    {
        public RoleCreateUpdateDtoValidator()
        {
            RuleFor(p => p.Name).NotEmpty().MaximumLength(15);

            RuleFor(p => p.Description).NotEmpty().MaximumLength(100);
        }
    }
}
