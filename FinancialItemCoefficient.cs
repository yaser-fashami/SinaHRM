using SinaHRM.Domain.Common;
using SinaHRM.Domain.ValueObjects;

namespace SinaHRM.Domain.Entities;

public class FinancialItemCoefficient : BaseEntity<short>
{
    private FinancialItemCoefficient()
    {
    }

    internal FinancialItemCoefficient(
        string fiscalYear,
        short version,
        byte financialItemDefinitionId,
        Money baseAmount,
        decimal increaseFactor,
        Money constantIncrease)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fiscalYear);

        if (version < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(version),
                "شماره نسخه باید بزرگ‌تر از صفر باشد.");
        }

        if (increaseFactor <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(increaseFactor),
                "ضریب افزایش باید بزرگ‌تر از صفر باشد.");
        }

        ArgumentNullException.ThrowIfNull(baseAmount);
        ArgumentNullException.ThrowIfNull(constantIncrease);

        FiscalYear = fiscalYear.Trim();
        Version = version;
        FinancialItemDefinitionId = financialItemDefinitionId;
        BaseAmount = baseAmount;
        IncreaseFactor = increaseFactor;
        ConstantIncrease = constantIncrease;
    }

    /// <summary>
    /// سال مالی
    /// </summary>
    public string FiscalYear { get; private set; } = null!;

    /// <summary>
    /// شماره بازنگری قواعد در همان سال مالی؛ از ۱ شروع می‌شود.
    /// </summary>
    public short Version { get; private set; }

    public byte FinancialItemDefinitionId { get; private set; }
    //Nav property
    public FinancialItemDefinition FinancialItemDefinition { get; private set; }

    /// <summary>
    /// مبلغ نهایی قلم مالی در سال مالی قبل.
    /// </summary>
    public Money BaseAmount { get; private set; } = null!;

    /// <summary>
    /// ضریب افزایش مبلغ پایه.
    /// </summary>
    public decimal IncreaseFactor { get; private set; } = 1.0m;

    /// <summary>
    /// مبلغ ثابت افزوده‌شده به حاصل اعمال ضریب.
    /// </summary>
    public Money ConstantIncrease { get; private set; } = null!;
}