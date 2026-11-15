using SinaHRM.Application.ContractManagement.Interfaces;
using SinaHRM.Domain.ValueObjects;
using System.Globalization;

namespace SinaHRM.Application.ContractManagement.Services;

public sealed class PersianDatePeriodService : IPersianDatePeriodService
{
    private readonly PersianCalendar _calendar = new();

    public PersianDatePeriod GetPeriod(DateTime date)
    {
        var year = _calendar.GetYear(date);
        var month = _calendar.GetMonth(date);

        var halfYear = month <= 6 ? 1 : 2;

        return new PersianDatePeriod(year, halfYear);
    }
}