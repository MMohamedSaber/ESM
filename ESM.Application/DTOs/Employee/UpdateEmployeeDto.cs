using ESM.Domain.Enums;

namespace ESM.Application.DTOs.Employee;

public class UpdateEmployeeDto
{
    public Guid DepartmentId { get; set; }
    public Guid PositionId { get; set; }
    public decimal Salary { get; set; }
    public EmployeeStatus Status { get; set; }
}
