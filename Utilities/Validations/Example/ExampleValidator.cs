using FluentValidation;
using Models.Dtos.Example;

namespace Utilities.Validations.Example
{
    public class ExampleValidator : BaseValidator<TestDto>
    {
        public ExampleValidator()
        {
            RuleFor(p => p.Name).NotEmpty().MaximumLength(100);
            RuleFor(p => p.Extra1).NotEmpty().MaximumLength(100);
            RuleFor(p => p.Extra2).NotEmpty().MaximumLength(100);
            //RuleFor(p => p.IsActive).Must(a => a is true or false); if your bool nullable
        }
    }
}
