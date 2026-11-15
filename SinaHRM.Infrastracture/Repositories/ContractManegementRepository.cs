using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SinaHRM.Application.Abstractions.Persistence;
using SinaHRM.Application.ContractManagement.DTOs;
using SinaHRM.Application.ContractManagement.Interfaces;
using SinaHRM.Application.Helpers;
using SinaHRM.Domain.Common;
using SinaHRM.Domain.Entities;
using SinaHRM.Domain.Exceptions;
using System.Data;
using System.Diagnostics.Contracts;

namespace SinaHRM.Infrastracture.Data.Sql.Repositories;

public class ContractManegementRepository : IContractManagementRepository
{
    private readonly SinaHRMDbContext _dbContext;
    private readonly IPersianDatePeriodService _periodService;

    public ContractManegementRepository(SinaHRMDbContext dbContext, IPersianDatePeriodService periodService)
    {
        _dbContext = dbContext;
        _periodService = periodService;
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
                employee.BirthCertificateNumber,
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
                            emp.BirthCertificateNumber,
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

    public async Task<Result> SaveContractTemplateAsync(string text, string code, short newVersion)
    {
        Result<EmploymentContractTemplateVersion> result;
        try
        {
            EmploymentContractTemplate template = await _dbContext.EmploymentContractTemplates.SingleAsync(c => c.Code == code);
            EmploymentContractTemplateVersion contract = new()
            {
                EmploymentContractTemplateId = template.Id,
                TemplateText = text,
                VersionNumber = newVersion
            };
            await _dbContext.EmploymentContractTemplateVersions.AddAsync(contract);
            await _dbContext.SaveChangesAsync();
            result = Result<EmploymentContractTemplateVersion>.Success(contract);
        }
        catch (Exception ex)
        {
            result = Result<EmploymentContractTemplateVersion>.Failure(new ExceptionError(ex));
        }

        return result;
    }

    public async Task<Result<IReadOnlyList<Employee>>> GetPersonnelsDetailByIdsAsync(IReadOnlyList<int> ids, CancellationToken cancellationToken)
    {
        Result<IReadOnlyList<Employee>> result;
        try
        {
            var res = await _dbContext.Emploies
                .Include(c=>c.JobLocation)
                .Include(c=>c.Membership)
                .Include(c=>c.Profession)
                .Include(c=>c.Department)
                .AsNoTracking()
                .Where(c => c.IsDelete == false && ids.Contains(c.Id))
                .ToListAsync(cancellationToken);

            if (res.Count > 0)
            {
                result = Result<IReadOnlyList<Employee>>.Success(res);
            }
            else
            {
                result = Result<IReadOnlyList<Employee>>.Failure(new EmptyListError());
            }
        }
        catch (Exception ex)
        {

            result = Result<IReadOnlyList<Employee>>.Failure(new Error(ex.HResult.ToString(), ex.Message, ex));
        }

        return result;
    }



    public async Task<Result<EmploymentContractTemplateVersion>> GetContractTemplateByCodeAsync(string code)
    {
        Result<EmploymentContractTemplateVersion> result;
        try
        {
            var contract = await _dbContext.EmploymentContractTemplateVersions
                            .Where(c => c.Template.Code == code)
                            .OrderByDescending(c => c.VersionNumber)
                            .FirstOrDefaultAsync();
            if (contract is not null)
            {
                result = Result<EmploymentContractTemplateVersion>.Success(contract);
            }
            else
            {
                result = Result<EmploymentContractTemplateVersion>.Failure(new ContractTemplateVersionDoesnotExistError());
            }
        }
        catch (Exception ex)
        {
            result = Result<EmploymentContractTemplateVersion>.Failure(new ExceptionError(ex));
        }

        return result;  
    }

    public async Task<Result<IReadOnlyList<ContractListItemDto>>> GetAllContractAsync(CancellationToken cancellationToken)
    {
        Result<IReadOnlyList<ContractListItemDto>> result;

        var contracts = await _dbContext.Set<EmploymentContract>()
            .AsNoTracking()
            .Where(contract => contract.IsDelete == false)
            .Include(contract => contract.Employee)
            .ThenInclude(contract => contract.Department)
            .Include(contract => contract.Employee)
            .ThenInclude(contract => contract.JobLocation)
            .Include(contract => contract.Employee)
            .ThenInclude(contract => contract.Membership)
            .ToListAsync(cancellationToken);

        if (contracts is not null)
        {
            contracts.ForEach(c =>
            {
                c.StartDate = DateConversionUtility.GregorianToPersianToDateOnly(c.StartDate);
                c.EndDate = DateConversionUtility.GregorianToPersianToDateOnly(c.EndDate.Value);
            });

            var contractsDto = contracts.Select(contract => new ContractListItemDto(
                    contract.Id,
                    contract.ContractNumber,
                    contract.Employee.FullName,
                    contract.StartDate,
                    contract.EndDate.Value,
                    contract.Status.ToString(),
                    contract.Employee.Department is null? "": contract.Employee.Department.DepartmentName,
                    contract.Employee.JobLocation.Branch.ToString(),
                    contract.Employee.Membership.CompanyName
                    )).ToList();

            result = Result<IReadOnlyList<ContractListItemDto>>.Success(contractsDto) ;
        }
        else
        {
            result = Result<IReadOnlyList<ContractListItemDto>>.Failure(
                        new Error(
                             "Contract.NotFound",
                             "Contract was not found."
                        )
                    );

        }

        return result;
    }

    public async Task<Result<string>> GetContractByIdAsync(int id, CancellationToken cancellationToken)
    {
        var res = await _dbContext.EmploymentContracts
            .AsNoTracking()
            .Where(c => c.Id == id && c.IsDelete == false)
            .SingleOrDefaultAsync(cancellationToken);

        if(res is not null)
        {
            return Result<string>.Success(res.FinalText);   
        }
        else
        {
            return Result<string>.Failure(new EmptyListError());
        }
    }

    public async Task<short[]> GetHRContractKPIsAsync()
    {
        short[] result = new short[3];
        try
        {
            result[0] = (short)await _dbContext.EmploymentContracts.Where(c => c.IsDelete == false).CountAsync(c => c.Status == Domain.Enums.EmploymentContractStatus.Draft);
            result[1] = (short)await _dbContext.EmploymentContracts.Where(c => c.IsDelete == false).CountAsync(c => c.Status == Domain.Enums.EmploymentContractStatus.Active);
            result[2] = (short)await _dbContext.EmploymentContracts.Where(c => c.IsDelete == false).CountAsync(c => c.Status == Domain.Enums.EmploymentContractStatus.PendingSignature);
        }
        catch (Exception ex)
        {
            result[0] = -1;
            result[1] = -1;
            result[2] = -1;
        }
        return result;
    }




    #region Private Methods
    #endregion
}
