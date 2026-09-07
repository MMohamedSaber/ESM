using FluentValidation;
using ESM.Application.DTOs.Organization;

namespace ESM.Application.Validators.Organization;

public class RegisterOrganizationDtoValidator : AbstractValidator<RegisterOrganizationDto>
{
    public RegisterOrganizationDtoValidator()
    {
        RuleFor(x => x.OrganizationNameEn)
            .NotEmpty().WithMessage("Organization name in English is required.")
            .MaximumLength(100);

        RuleFor(x => x.OrganizationNameAr)
            .NotEmpty().WithMessage("Organization name in Arabic is required.")
            .MaximumLength(100);

        RuleFor(x => x.AdminName)
            .NotEmpty().WithMessage("Admin name is required.")
            .MaximumLength(100);

        RuleFor(x => x.AdminEmail)
            .NotEmpty().WithMessage("Admin email is required.")
            .EmailAddress().WithMessage("Invalid email format.");

        RuleFor(x => x.AdminPassword)
            .NotEmpty().WithMessage("Admin password is required.")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long.");
    }
}
