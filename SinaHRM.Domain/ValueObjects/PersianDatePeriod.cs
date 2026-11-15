namespace SinaHRM.Domain.ValueObjects;

public sealed record PersianDatePeriod
{
    public int Year { get; }
    public int HalfYear { get; }

    public string PeriodCode => $"{Year}{HalfYear}";

    public PersianDatePeriod(int year, int halfYear)
    {
        if (year <= 0)
            throw new ArgumentOutOfRangeException(nameof(year));

        if (halfYear is not (1 or 2))
            throw new ArgumentOutOfRangeException(nameof(halfYear));

        Year = year;
        HalfYear = halfYear;
    }
}
