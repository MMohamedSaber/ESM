using ESM.Application.DTOs;
using FluentValidation;

namespace ESM.Application.Validators;

public class CreateServiceCategoryDtoValidator : AbstractValidator<CreateServiceCategoryDto>
{
    public CreateServiceCategoryDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required.")
            .MaximumLength(200).WithMessage("Name cannot exceed 200 characters.");
    }
}
