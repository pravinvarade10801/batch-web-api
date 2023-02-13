using batch_webapi.Data.ViewModels;
using FluentValidation;

namespace batch_webapi.Data.Validators
{
    public class AttributesValidator:AbstractValidator<AttributesVM>
    {
        public AttributesValidator()
        {
            RuleFor(u => u.Key).NotNull().NotEmpty();
            RuleFor(u => u.Value).NotNull().NotEmpty();
        }
       
    }
}
