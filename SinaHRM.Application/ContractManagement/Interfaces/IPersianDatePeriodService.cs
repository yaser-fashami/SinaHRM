using SinaHRM.Domain.ValueObjects;

namespace SinaHRM.Application.ContractManagement.Interfaces;

public interface IPersianDatePeriodService
{
    PersianDatePeriod GetPeriod(DateTime date);
}