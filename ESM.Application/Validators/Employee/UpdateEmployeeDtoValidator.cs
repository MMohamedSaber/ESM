using ESM.Application.DTOs.Employee;
using FluentValidation;

namespace ESM.Application.Validators.Employee;

public class UpdateEmployeeDtoValidator : AbstractValidator<UpdateEmployeeDto>
{
    public UpdateEmployeeDtoValidator()
    {
        RuleFor(x => x.DepartmentId)
            .NotEmpty().WithMessage("Department ID is required.");

        RuleFor(x => x.PositionId)
            .NotEmpty().WithMessage("Position ID is required.");

        RuleFor(x => x.Salary)
            .GreaterThan(0).WithMessage("Salary must be greater than zero.");

        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Invalid status.");
    }
}
