using SinaHRM.Domain.Common;
using SinaHRM.Domain.Enums;
using SinaHRM.Domain.ValueObjects;

namespace SinaHRM.Domain.Entities;

public class FinancialItemDefinition : AggregateRoot<byte>
{
    private readonly List<FinancialItemCoefficient> _financialItemCoefficients = [];
    private readonly List<EmploymentContractFinancialItem> _contractItems = [];

    private FinancialItemDefinition()
    {
    }

    // شناسه پایدار و یکتای قلم مالی
    public string Code { get; private set; } = null!;

    public string Title { get; private set; } = null!;

    public FinancialItemType ItemType { get; private set; }

    public bool AllowManualOverride { get; private set; }

    public byte DisplayOrder { get; private set; }

    // نسخه‌های ضرایب قلم مالی
    public IReadOnlyCollection<FinancialItemCoefficient> FinancialItemCoefficients
        => _financialItemCoefficients.AsReadOnly();

    // اقلام مالی مرتبط با قراردادهای استخدامی
    public IReadOnlyCollection<EmploymentContractFinancialItem> ContractItems
        => _contractItems.AsReadOnly();

    /// <summary>
    /// ایجاد نسخه جدید ضرایب برای یک سال مالی.
    /// نسخه جدید بلافاصله معتبر خواهد بود.
    /// </summary>
    public FinancialItemCoefficient AddCoefficient(
        string fiscalYear,
        Money baseAmount,
        decimal increaseFactor,
        Money constantIncrease)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(fiscalYear);

        fiscalYear = fiscalYear.Trim();

        var lastVersion = _financialItemCoefficients
            .Where(x => x.FiscalYear == fiscalYear)
            .Select(x => (int)x.Version)
            .DefaultIfEmpty(0)
            .Max();

        if (lastVersion >= short.MaxValue)
        {
            throw new InvalidOperationException(
                $"امکان ایجاد نسخه جدید برای سال مالی {fiscalYear} وجود ندارد.");
        }

        var coefficient = new FinancialItemCoefficient(
            fiscalYear,
            checked((short)(lastVersion + 1)),
            Id,
            baseAmount,
            increaseFactor,
            constantIncrease);

        _financialItemCoefficients.Add(coefficient);

        return coefficient;
    }
}