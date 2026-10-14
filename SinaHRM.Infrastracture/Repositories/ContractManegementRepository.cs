using Microsoft.EntityFrameworkCore;
using SinaHRM.Application.Abstractions.Persistence;
using SinaHRM.Application.ContractManagement.DTOs;
using SinaHRM.Domain.Entities;
using SinaHRM.Utilities;

namespace SinaHRM.Infrastracture.Data.Sql.Repositories;

public class ContractManegementRepository : IContractManagementRepository
{
    private readonly SinaHRMDbContext _dbContext;

    public ContractManegementRepository(SinaHRMDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<IReadOnlyList<PersonnelListItemDto>>> GetAllPersonnelAsync(CancellationToken cancellationToken)
    {
        Result<IReadOnlyList<PersonnelListItemDto>> result;

        var personnels = await _dbContext.Set<Employee>()
             .AsNoTracking()
             .Where(employee => employee.IsDelete == false)
             .Include(employee => employee.Profession)
             .Include(employee => employee.JobLocation)
             .Include(employee => employee.Membership)
             .Include(employee => employee.Department)
             .Select(employee => new PersonnelListItemDto(
                employee.Id,
                employee.PersonnelCode,
                employee.FullName,
                employee.Photos.FirstOrDefault().Data,
                employee.Age,
                employee.BirthDate,
                employee.FatherName,
                employee.NationalCode,
                employee.PlaceBirth,
                employee.PlaceOfIssuanceOfBirthCertificate,
                employee.Gneder.ToString(),
                employee.EducationalQualification.ToString(),
                employee.MobileNumber,
                employee.PhoneNumber,
                employee.Email,
                employee.Address,
                employee.PostalCode,
                employee.Profession.JobName + " / " + employee.JobLocation.JobLocationName,
                employee.Membership.CompanyName,
                employee.Department != null ? employee.Department.DepartmentName : "???",
                employee.ManagementLayer != null ? employee.ManagementLayer.ToString() : null,
                employee.MaritalStatus.ToString(),
                employee.NumberOfChildren,
                employee.TotalSalary
        )).ToListAsync(cancellationToken);

        if (personnels is not null)
        {
            result = Result<IReadOnlyList<PersonnelListItemDto>>.Success(personnels);
        }
        else
        {
            result = Result<IReadOnlyList<PersonnelListItemDto>>.Failure(
                new Error(
                     "Personnel.NotFound",
                     "Personnel was not found."
                )
            );
        }
        return result;
    }

    public async Task<Result<PersonnelListItemDto>> GetPersonnelDetailByIdAsync(int id)
    {
        Result<PersonnelListItemDto> result;

        var personnel = await _dbContext.Set<Employee>()
                .AsNoTracking()
                .Where(c => c.Id == id && c.IsDelete == false)
                .Select(emp => new PersonnelListItemDto(
                            emp.Id,
                            emp.PersonnelCode,
                            emp.FullName,
                            emp.Photos.FirstOrDefault().Data,
                            emp.Age,
                            emp.BirthDate,
                            emp.FatherName,
                            emp.NationalCode,
                            emp.PlaceBirth,
                            emp.PlaceOfIssuanceOfBirthCertificate,
                            emp.Gneder.ToString(),
                            emp.EducationalQualification.ToString(),
                            emp.MobileNumber,
                            emp.PhoneNumber,
                            emp.Email,
                            emp.Address,
                            emp.PostalCode,
                            emp.Profession.JobName + '/' + emp.JobLocation.JobLocationName,
                            emp.Membership.CompanyName,
                            emp.Department != null ? emp.Department.DepartmentName : "???",
                            emp.ManagementLayer != null ? emp.ManagementLayer.ToString() : null,
                            emp.MaritalStatus.ToString(),
                            emp.NumberOfChildren,
                            emp.TotalSalary
                )).SingleAsync();

        if (personnel is not null)
        {
            result = Result<PersonnelListItemDto>.Success(personnel);
        }
        else
        {
            result = Result<PersonnelListItemDto>.Failure(
            new Error(
                 "Personnel.NotFound",
                 "Personnel was not found."));
        }

        return result; 
    }
}
