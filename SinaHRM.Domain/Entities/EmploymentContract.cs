using SinaHRM.Domain.Common;
using SinaHRM.Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace SinaHRM.Domain.Entities;

public class EmploymentContract : BaseEntity<int>
{
    public int EmployeeId { get; set; }
    //Nav Property
    public Employee Employee { get; set; } = null!;

    [NotMapped]
    public Employee? DisplayEmployee { get; set; }

    public short EmploymentContractTemplateVersionId { get; set; }
    //Nav Property
    public EmploymentContractTemplateVersion TemplateVersion { get; set; }  = null!;

    public string ContractNumber { get; set; } = null!;

    public EmploymentContractType ContractType { get; set; }

    public EmploymentContractStatus Status { get; set; } = EmploymentContractStatus.Draft;

    public DateOnly StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    // متن نهایی قرارداد که برای امضا ارسال می‌شود.
    public string FinalText { get; set; } = null!;

    // شناسه فرایند در WorkflowEngine؛ قالب آن باید با قرارداد واقعی API هماهنگ شود.
    public string? WorkflowInstanceId { get; set; }

    public DateTimeOffset? SignatureCompletedAtUtc { get; set; }

    public DateTimeOffset? ActivatedAtUtc { get; set; }

    public DateTimeOffset? CancelledAtUtc { get; set; }

    public string? CancellationReason { get; set; }


    //Nav Property
    public ICollection<EmploymentContractFinancialItem> FinancialItems { get; set; }  = new List<EmploymentContractFinancialItem>();
}