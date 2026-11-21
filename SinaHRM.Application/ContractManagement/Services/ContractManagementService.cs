using SinaHRM.Application.Abstractions.Persistence;
using SinaHRM.Application.ContractManagement.DTOs;
using SinaHRM.Application.ContractManagement.Interfaces;
using SinaHRM.Application.Helpers;
using SinaHRM.Domain.Common;
using SinaHRM.Domain.Entities;
using SinaHRM.Domain.Enums;
using SinaHRM.Domain.Exceptions;
using System.Diagnostics.Contracts;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace SinaHRM.Application.ContractManagement.Services;

public class ContractManagementService : IContractManagementService
{
    private readonly IContractManagementRepository _CMRepo;
    private readonly IEmploymentContractRepository _EMRepo;
    private readonly IPersianDatePeriodService _PDService;
    private readonly IHtmlTemplateService _HTMLTempService;

    public ContractManagementService(
        IContractManagementRepository cmRepo,
        IEmploymentContractRepository emRepo,
        IPersianDatePeriodService pDService,
        IHtmlTemplateService hTMLTempService)
    {
        _CMRepo = cmRepo;
        _EMRepo = emRepo;
        _PDService = pDService;
        _HTMLTempService = hTMLTempService;
    }

    #region Personnel
    public async Task<Result<IReadOnlyList<PersonnelListItemDto>>> GetAllPersonnelAsync(CancellationToken cancellationToken)
    {
        return await _CMRepo.GetAllPersonnelAsync(cancellationToken);
    }


    public async Task<Result<PersonnelListItemDto>> GetPersonnelDetailByIdAsync(int id)
    {
        return await _CMRepo.GetPersonnelDetailByIdAsync(id);
    }

    #endregion

    #region Contract Template
    public async Task<Result> SaveContractTemplate(JsonDocument input)
    {

        JsonElement root = input.RootElement;
        string code = root.GetProperty("code").GetString()!;
        string text = root.GetProperty("text").GetString()!;

        var originalContractTemplate = await _CMRepo.GetContractTemplateByCodeAsync(code);
        if (originalContractTemplate.IsSuccess && originalContractTemplate.Value.TemplateText != text)
        {
            return await _CMRepo.SaveContractTemplateAsync(text, code, ++originalContractTemplate.Value.VersionNumber);
        }
        else if (originalContractTemplate.IsFailure && originalContractTemplate.Error.Code == "101")
        {
            return await _CMRepo.SaveContractTemplateAsync(text, code, 1);
        }
        else
        {
            return Result.Failure(new Error("102", "contract template exist and nothing has changed"));
        }

    }

    public async Task<Result<EmploymentContractTemplateVersion>> GetEmploymentContractTemplate(string code)
    {
        return await _CMRepo.GetContractTemplateByCodeAsync(code);
    }

    #endregion

    #region Contract
    public async Task<Result> CreateContract(IReadOnlyList<int> employeeIds, CancellationToken cancellationToken)
    {
        Result result;
        List<Result<EmploymentContract>> contracts = new();
        Result<EmploymentContract> contractWithNumber;

        var emploies = await _CMRepo.GetPersonnelsDetailByIdsAsync(employeeIds, cancellationToken);
        if (emploies.IsSuccess)
        {
            Result<EmploymentContractTemplateVersion> tempVersionTH = await _CMRepo.GetContractTemplateByCodeAsync("CONTRACT_CENTER");
            Result<EmploymentContractTemplateVersion> tempVersionBN = await _CMRepo.GetContractTemplateByCodeAsync("CONTRACT_BANDAR");
            var defaultStartDate = DateConversionUtility.PersianPeriodToGregorianRange(_PDService.GetPeriod(DateTime.Now)).StartDate;

            foreach (var employee in emploies.Value)
            {
                EmploymentContract contract = new();
                var lastContractEndDate = await _EMRepo.GetLatestValidContractEndDateAsync(employee.Id, cancellationToken);
                if (lastContractEndDate is null)
                {
                    lastContractEndDate = defaultStartDate;
                }
                else
                {
                    lastContractEndDate = lastContractEndDate.Value.AddDays(1);
                }
                contract.EmployeeId = employee.Id;
                contract.DisplayEmployee = new Employee
                {
                    FirstName = employee.FirstName,
                    LastName = employee.LastName,
                };
                contract.StartDate = lastContractEndDate.Value;
                contract.ContractType = EmploymentContractType.FixedTerm;
                if (contract.StartDate.Month == 9 && contract.StartDate.Day == 23)
                {
                    contract.EndDate = new(contract.StartDate.Year + 1, 3, 21);
                }else if (contract.StartDate.Month == 3 && contract.StartDate.Day == 20)
                {
                    contract.EndDate = new(contract.StartDate.Year, 9, 23);
                }
                else
                {
                    contract.EndDate = lastContractEndDate.Value.AddMonths(6);
                }

                var parametersContract = CreateParametersContract(employee, contract);

                if (employee.JobLocation.Branch == Branch.headquarter)
                {
                    contract.EmploymentContractTemplateVersionId = tempVersionTH.Value.Id;
                    contract.ContractNumber = await _EMRepo.GetContractNumberAsync(true, lastContractEndDate.Value);
                    contract.FinalText = _HTMLTempService.Render(tempVersionTH.Value.TemplateText, parametersContract);
                }
                else
                {
                    contract.EmploymentContractTemplateVersionId = tempVersionBN.Value.Id;
                    contract.ContractNumber = await _EMRepo.GetContractNumberAsync(false, lastContractEndDate.Value);
                    contract.FinalText = _HTMLTempService.Render(tempVersionBN.Value.TemplateText, parametersContract);
                }
                
                contractWithNumber = await _EMRepo.SaveContractWithGeneratedContractNumberAsync(contract, cancellationToken);

                if (contractWithNumber.IsFailure)
                {
                    contracts.Add(Result<EmploymentContract>.Failure(contractWithNumber.Error));
                }
                else
                {
                    contracts.Add(contractWithNumber);
                }

            }

            StringBuilder msg = new();
            if (contracts.Count(c => c.IsSuccess) > 0)
            {
                msg.Append($" تعداد {contracts.Count(c => c.IsSuccess)} .قرارداد  در وضعیت پیش نویس ثبت شد");
                msg.Append("<br/>");
            }
            foreach (var contract in contracts)
            {
                if (contract.IsFailure)
                {
                    msg.Append("<Error/>");
                    msg.Append(contract.Error.Message);
                    msg.Append("<br/>");
                }
                else
                {
                    msg.Append(contract.Message);
                    msg.Append("<br/>");
                }
            }

            result = Result.Success(msg.ToString());
        }
        else
        {
            result = Result.Failure(emploies.Error);
        }

        return result;
    }

    public async Task<Result<IReadOnlyList<ContractListItemDto>>> GetAllContractAsync(CancellationToken cancellationToken)
    {
        return await _CMRepo.GetAllContractAsync(cancellationToken);
    }

    public async Task<Result<string>> GetContractById(int id, CancellationToken cancellationToken)
    {
        return await _CMRepo.GetContractByIdAsync(id, cancellationToken);
    }

    public async Task<Result<short[]>> GetHRContractKPIs()
    {
        Result<short[]> result;
        var res = await _CMRepo.GetHRContractKPIsAsync();
        if (res.All(c => c != -1))
        {
            result = Result<short[]>.Success(res);
        }
        else
        {
            result = Result<short[]>.Failure(new CountOperationError());
        }

        return result;
    }
    #endregion



    #region Private Methods

    private static Dictionary<string, string> CreateParametersContract(Employee employee, EmploymentContract contract)
    {
        Dictionary<string, string> financialItems = new()
        {
            { "Membership.CompanyName", employee.Membership.CompanyName },
            { "Membership.CEO", employee.Membership.CEO is null ? " " : employee.Membership.CEO },
            { "Membership.Address", employee.Membership.Address is null ? " " : employee.Membership.Address },
            { "Membership.PostalCode", employee.Membership.PostalCode is null ? " " : employee.Membership.PostalCode },
            { "Membership.CEOMobile", employee.Membership.CEOMobile is null ? " " : employee.Membership.CEOMobile },
            { "Employee.FullName", employee.FullName },
            { "Employee.PersonnelCode", employee.PersonnelCode },
            { "Employee.FatherName", employee.FatherName },
            { "Employee.NationalCode", employee.NationalCode },
            { "Employee.MobileNumber", employee.MobileNumber is null ? " " : employee.MobileNumber },
            { "Employee.BirthCetificateNumber", employee.BirthCertificateNumber.Length == 0 ? " ": employee.BirthCertificateNumber },
            { "Employee.PlaceOfIssueanceOfBirthCertification", employee.PlaceOfIssuanceOfBirthCertificate },
            { "Employee.PlaceBirth", employee.PlaceBirth },
            { "Employee.NumberOfChildren", employee.NumberOfChildren is null ? " " : employee.NumberOfChildren.Value.ToString() },
            { "Employee.Address", employee.Address is null ? " " : employee.Address },
            { "Employee.BirthDate", employee.BirthDate is null ? " " : DateConversionUtility.GregorianToPersian(employee.BirthDate.Value) },
            { "Employee.MaritalStatus", FarsiConverter.ConvertMaritalStatusToFarsi(employee.MaritalStatus.ToString()) },
            { "JobLocation.Branch", employee.JobLocation.JobLocationName },
            { "Profession.jobName", employee.Profession.JobName },
            { "EmploymentContract.EmploymentContractType", FarsiConverter.ConvertEmploymentContractTypeToFarsi(contract.ContractType.ToString()) },
            { "EmploymentContract.ContractNumber", contract.ContractNumber },
            { "EmploymentContract.StartDate", DateConversionUtility.GregorianToPersian(contract.StartDate) },
            { "EmploymentContract.EndDate", DateConversionUtility.GregorianToPersian(contract.EndDate.Value) },
            { "EmploymentContract.SignatureCompleteAtUtc", contract.SignatureCompletedAtUtc is null? " ": DateConversionUtility.GregorianToPersian(contract.SignatureCompletedAtUtc.Value.DateTime) },
            { "Employee.BaseSalary", employee.BaseSalary.ToString("N0", CultureInfo.InvariantCulture) },
            { "Employee.YearsBase", employee.YearsBase?.ToString("N0", CultureInfo.InvariantCulture) ?? "0" },
            { "Employee.HousingBonous", employee.HousingBonous?.ToString("N0", CultureInfo.InvariantCulture) ?? "0" },  
            { "Employee.WorkerBonous", employee.WorkerBonous?.ToString("N0", CultureInfo.InvariantCulture) ?? "0" },
            { "Employee.ResponsibilitySalary", employee.ResponsibilitySalary?.ToString("N0", CultureInfo.InvariantCulture) ?? "0" },
            { "Employee.MartialBonous", employee.MartialBonous?.ToString("N0", CultureInfo.InvariantCulture) ?? "0"  },
            { "Employee.ChildrenBonous", employee.ChildrenBonous?.ToString("N0", CultureInfo.InvariantCulture) ?? "0" },
            { "Employee.Total", employee.TotalSalary.ToString("N0", CultureInfo.InvariantCulture) }
        };

        return financialItems;
    }




    #endregion
}
