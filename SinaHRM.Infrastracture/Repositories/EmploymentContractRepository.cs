using Microsoft.EntityFrameworkCore;
using SinaHRM.Application.ContractManagement.Interfaces;
using SinaHRM.Domain.Common;
using SinaHRM.Domain.Entities;
using SinaHRM.Domain.Enums;
using SinaHRM.Domain.Exceptions;
using SinaHRM.Infrastracture;
using System.Data;

public sealed class EmploymentContractRepository : IEmploymentContractRepository
{
    private readonly SinaHRMDbContext _context;
    private readonly IPersianDatePeriodService _periodService;

    public EmploymentContractRepository(SinaHRMDbContext context, IPersianDatePeriodService periodService)
    {
        _context = context;
        _periodService = periodService;
    }


    public async Task<string> GetContractNumberAsync(bool isTehran, DateOnly contractDate, CancellationToken cancellationToken = default)
    {
        var period = _periodService.GetPeriod(contractDate.ToDateTime(new TimeOnly(0, 0)));
        var periodCode = period.PeriodCode;

        var lastSequence = await _context.EmploymentContracts
            .Where(x =>
                x.ContractNumber != null &&
                x.ContractNumber.Length == 13 &&
                x.ContractNumber.Substring(2, 5) == periodCode)
            .Select(x => x.ContractNumber!.Substring(8, 5))
            .OrderByDescending(x => x)
            .FirstOrDefaultAsync(cancellationToken);

        var sequence = lastSequence is null
            ? 1
            : int.Parse(lastSequence) + 1;

        if (sequence > 99999)
        {
            throw new InvalidOperationException(
                "ظرفیت شماره‌گذاری قراردادها در این دوره تکمیل شده است.");
        }

        var prefix = isTehran ? "TH" : "BN";


        return $"{prefix}{periodCode}-{sequence:D5}";
    }

    public async Task<DateOnly?> GetLatestValidContractEndDateAsync(int employeeId, CancellationToken cancellationToken = default)
    {
        return await _context.EmploymentContracts
            .AsNoTracking()
            .Where(x =>
                x.EmployeeId == employeeId &&
                (x.Status == EmploymentContractStatus.Active ||
                 x.Status == EmploymentContractStatus.Expired) && x.EndDate != null)
            .OrderByDescending(x => x.EndDate)
            .Select(x => x.EndDate)
            .FirstOrDefaultAsync(cancellationToken);
    }


    public async Task<Result<EmploymentContract>> SaveContractWithGeneratedContractNumberAsync(EmploymentContract contract, CancellationToken cancellationToken = default)
    {
        Result<EmploymentContract> result;
        try
        {
            var hasInProcessContract = await _context.EmploymentContracts
                                            .AnyAsync(x =>
                                                x.EmployeeId == contract.EmployeeId &&
                                                (x.Status == EmploymentContractStatus.Draft ||
                                                 x.Status == EmploymentContractStatus.PendingSignature ||
                                                 x.Status == EmploymentContractStatus.Signed ||
                                                 x.Status == EmploymentContractStatus.Rejected),
                                                 cancellationToken);

            if (hasInProcessContract)
            {
                result = Result<EmploymentContract>.Failure(new InProcessContractExistError(contract.ContractNumber.Substring(2, 5), contract.DisplayEmployee?.FullName));
                return result;
            }
            await _context.EmploymentContracts.AddAsync(contract, cancellationToken);

            await _context.SaveChangesAsync(cancellationToken);
            result = Result<EmploymentContract>.Success(contract, $" قرارداد به شماره {contract.ContractNumber} برای {contract.DisplayEmployee?.FullName} در وضعیت پیش نویس ثبت شد");
        }
        catch (Exception ex)
        {
            result = Result<EmploymentContract>.Failure(new ExceptionError(ex));
        }

        return result;
    }


}