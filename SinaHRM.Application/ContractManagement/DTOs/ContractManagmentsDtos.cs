using SinaHRM.Domain.Entities;
using SinaHRM.Domain.Enums;

namespace SinaHRM.Application.ContractManagement.DTOs;

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
    string job,
    string Membership,
    string Department,
    string? ManagementLayer,
    string MaritalStatus,
    byte? NumberOfChildren,
    decimal TotalSalary);

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



