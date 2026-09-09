namespace ESM.Application.DTOs.Employee;

public class CreateEmployeeDto
{
    public string Name { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public Guid DepartmentId { get; set; }
    public Guid PositionId { get; set; }
    public decimal Salary { get; set; }
}
