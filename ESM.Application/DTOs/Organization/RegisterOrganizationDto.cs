namespace ESM.Application.DTOs.Organization;

public class RegisterOrganizationDto
{
    public string OrganizationNameEn { get; set; } = string.Empty;
    public string OrganizationNameAr { get; set; } = string.Empty;
    public string AdminName { get; set; } = string.Empty;
    public string AdminEmail { get; set; } = string.Empty;
    public string AdminPassword { get; set; } = string.Empty;
}
