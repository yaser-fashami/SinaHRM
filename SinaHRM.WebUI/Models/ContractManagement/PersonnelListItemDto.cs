
namespace SinaHRM.WebUI.Models.ContractManagement;

public sealed record PersonnelListItemDto(
    int Id,
    string PersonnelCode,
    string FullName,
    byte[]? Photo,
    byte? Age,
    DateTime? BirthDate,
    string FatherName,
    string NationalCode,
    string BirthCertificateNumber,
    string PlaceBirth,
    string PlaceOfIssuanceOfBirthCertificate,
    string Gender,
    string EducationalQualification,
    string? MobileNumber,
    string? PhoneNumber,
    string? Email,
    string? Address,
    string? PostalCode,
    string Job,
    string Membership,
    string Department,
    string? ManagementLayer,
    string MaritalStatus,
    byte? NumberOfChildren,
    decimal TotalSalary);
