
using SinaHRM.Domain.ValueObjects;
using System.Globalization;
using System.Text;

namespace SinaHRM.Application.Helpers;

public static class DateConversionUtility
{
    #region Persian To Gregorian

    /// <summary>
    /// Converts a Persian date string to Gregorian DateTime.
    /// Supported formats: 1405/07/10, 1405-07-10, 14050710
    /// Persian and Arabic digits are also supported.
    /// </summary>
    public static DateTime PersianToGregorian(string persianDate)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(persianDate);

        var normalized = NormalizeDate(persianDate);
        var digits = GetDigits(normalized);

        if (digits.Length != 8)
            throw new FormatException("Invalid Persian date format.");

        var year = int.Parse(digits[..4]);
        var month = int.Parse(digits.Substring(4, 2));
        var day = int.Parse(digits.Substring(6, 2));

        var calendar = new PersianCalendar();

        return calendar.ToDateTime(
            year, month, day,
            0, 0, 0, 0);
    }

    public static DateTime PersianToGregorian(int persianDate)
        => PersianToGregorian(persianDate.ToString("D8"));

    public static DateTime PersianToGregorian(long persianDate)
        => PersianToGregorian(persianDate.ToString("D8"));

    public static DateOnly PersianToGregorianDateOnly(string persianDate)
        => DateOnly.FromDateTime(PersianToGregorian(persianDate));

    public static DateOnly PersianToGregorianDateOnly(int persianDate)
        => DateOnly.FromDateTime(PersianToGregorian(persianDate));

    public static string PersianPeriodToString(PersianDatePeriod period)
    {
        ArgumentNullException.ThrowIfNull(period);

        return $"{period.Year:0000}{period.HalfYear}";
    }
    #endregion

    #region Gregorian To Persian

    /// <summary>
    /// Converts Gregorian DateTime to Persian date string.
    /// Output example: 1405/07/10
    /// </summary>
    public static string GregorianToPersian(DateTime gregorianDate)
    {
        var calendar = new PersianCalendar();

        var year = calendar.GetYear(gregorianDate);
        var month = calendar.GetMonth(gregorianDate);
        var day = calendar.GetDayOfMonth(gregorianDate);

        return $"{year:0000}/{month:00}/{day:00}";
    }

    public static string GregorianToPersian(DateOnly gregorianDate)
        => GregorianToPersian(gregorianDate.ToDateTime(TimeOnly.MinValue));

    public static DateOnly GregorianToPersianToDateOnly(DateOnly gregorianDate)
    {
        return DateOnly.Parse(GregorianToPersian(gregorianDate.ToDateTime(TimeOnly.MinValue)));
    }

    public static string GregorianToPersian(string gregorianDate,  string format = "yyyy/MM/dd")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(gregorianDate);

        var normalized = NormalizeDate(gregorianDate);

        var parsed = DateTime.ParseExact(
            normalized,
            new[]
            {
                "yyyy/MM/dd",
                "yyyy-MM-dd",
                "yyyyMMdd",
                "yyyy.MM.dd"
            },
            CultureInfo.InvariantCulture,
            DateTimeStyles.None);

        return GregorianToPersian(parsed);
    }

    public static string GregorianToPersian(int gregorianDate)
        => GregorianToPersian(gregorianDate.ToString("D8"));

    public static string GregorianToPersian(long gregorianDate)
        => GregorianToPersian(gregorianDate.ToString("D8"));

    public static (DateOnly StartDate, DateOnly EndDate) PersianPeriodToGregorianRange(PersianDatePeriod period)
    {
        ArgumentNullException.ThrowIfNull(period);

        var calendar = new PersianCalendar();

        var startMonth = period.HalfYear == 1 ? 1 : 7;
        var endMonth = period.HalfYear == 1 ? 6 : 12;

        var startDate = calendar.ToDateTime(
            period.Year, startMonth, 1, 0, 0, 0, 0);

        var endDate = calendar.ToDateTime(
            period.Year,
            endMonth,
            calendar.GetDaysInMonth(period.Year, endMonth),
            0, 0, 0, 0);

        return (
            DateOnly.FromDateTime(startDate),
            DateOnly.FromDateTime(endDate)
        );
    }
    #endregion

    #region Persian Formatting

    public static int GregorianToPersianNumber(DateTime date)
        => int.Parse(GregorianToPersian(date).Replace("/", ""));

    public static long GregorianToPersianLong(DateTime date)
        => long.Parse(GregorianToPersian(date).Replace("/", ""));

    #endregion

    #region Helpers

    private static string NormalizeDate(string value)
    {
        var builder = new StringBuilder(value.Length);

        foreach (var ch in value)
        {
            builder.Append(ch switch
            {
                >= '\u06F0' and <= '\u06F9' =>
                    (char)('0' + ch - '\u06F0'),

                >= '\u0660' and <= '\u0669' =>
                    (char)('0' + ch - '\u0660'),

                _ => ch
            });
        }

        return builder.ToString().Trim();
    }

    private static string GetDigits(string value)
    {
        return new string(value.Where(char.IsDigit).ToArray());
    }

    #endregion
}

