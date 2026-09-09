using ESM.Domain.Enums;

namespace ESM.Application.DTOs.Employee;

public class EmployeeDto
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public string EmployeeNumber { get; set; } = string.Empty;
    public Guid DepartmentId { get; set; }
    public Guid PositionId { get; set; }
    public DateTime HireDate { get; set; }
    public decimal Salary { get; set; }
    public EmployeeStatus Status { get; set; }
}
