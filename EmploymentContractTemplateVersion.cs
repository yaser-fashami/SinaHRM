using SinaHRM.Domain.Common;

namespace SinaHRM.Domain.Entities;

public class EmploymentContractTemplateVersion : BaseEntity<short>
{

    public short VersionNumber { get; set; }

    public string TemplateText { get; set; } = null!;

    public string? ChangeDescription { get; set; }

    public int? CreatedByEmployeeId { get; set; }

    public byte EmploymentContractTemplateId { get; set; }

    //Nav Property
    public EmploymentContractTemplate Template { get; set; } = null!;
    //Nav Property
    public ICollection<EmploymentContract> Contracts { get; set; } = new List<EmploymentContract>();
}