using SinaHRM.Domain.Common;
using SinaHRM.Domain.Enums;
using SinaHRM.Domain.ValueObjects;

namespace SinaHRM.Domain.Entities;

public class EmploymentContractFinancialItem : BaseEntity<short>
{
    private EmploymentContractFinancialItem()
    {
        // موردنیاز EF Core
    }

    public int EmploymentContractId { get; private set; }
    public EmploymentContract Contract { get; private set; } = null!;

    // در صورت استفاده از کاتالوگ مرکزی، مقدار دارد.
    public byte? FinancialItemDefinitionId { get; private set; }
    public FinancialItemDefinition? Definition { get; private set; }

    // Snapshot مشخصات قلم مالی در زمان ثبت قرارداد
    public string Code { get; private set; } = null!;

    public string Title { get; private set; } = null!;

    public FinancialItemType ItemType { get; private set; }

    // مبلغی که سامانه محاسبه کرده است.
    public Money CalculatedAmount { get; private set; } = Money.Zero();

    // مبلغ نهایی مورد توافق در قرارداد.
    public Money FinalAmount { get; private set; } = Money.Zero();

    public bool IsManuallyOverridden { get; private set; }

    public string? OverrideReason { get; private set; }

    public byte DisplayOrder { get; private set; }

    public int? UpdatedByEmployeeId { get; private set; }

    public void SetCalculatedAmount(Money amount)
    {
        CalculatedAmount = amount;

        // تا زمانی که مبلغ دستی اصلاح نشده، مبلغ نهایی نیز به‌روز می‌شود.
        if (!IsManuallyOverridden)
        {
            FinalAmount = amount;
        }
    }

    public void OverrideFinalAmount(Money amount, string reason, int updatedByEmployeeId, DateTimeOffset updatedAtUtc)
    {
        if (string.IsNullOrWhiteSpace(reason))
        {
            throw new ArgumentException(
                "دلیل اصلاح مبلغ الزامی است.",
                nameof(reason));
        }

        if (Definition is not null && !Definition.AllowManualOverride)
        {
            throw new InvalidOperationException(
                "اصلاح دستی مبلغ این قلم مجاز نیست.");
        }

        FinalAmount = amount;
        IsManuallyOverridden = true;
        OverrideReason = reason.Trim();
        UpdatedByEmployeeId = updatedByEmployeeId;
        ModifiedAt = updatedAtUtc;
    }


}