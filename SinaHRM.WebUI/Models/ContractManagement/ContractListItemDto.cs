namespace SinaHRM.WebUI.Models.ContractManagement;

public sealed record ContractListItemDto(
    int Id,
    string ContractNumber,
    string EmployeeFullName,
    DateOnly StartDate,
    DateOnly EndDate,
    string Status,
    string Departman,
    string Branch,
    string Membership);
