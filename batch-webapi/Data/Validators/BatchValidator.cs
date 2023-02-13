using batch_webapi.Data.ViewModels;
using FluentValidation;

namespace batch_webapi.Data.Validators
{
    public class BatchValidator:AbstractValidator<BatchVM>
    {
        public BatchValidator()
        {
            RuleFor(u => u.BusinessUnit).NotNull().NotEmpty();
            RuleFor(model => model.Attributes)
                .NotNull()
                .SetValidator(new AttributesValidator());
        }
    }
}
