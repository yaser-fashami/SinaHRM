using SinaHRM.Domain.Common;

namespace SinaHRM.Domain.Entities;

public class EmploymentContractTemplate : BaseEntity<byte>
{
    /// <summary>
    /// نشان دهنده نوع قرار داد برای بندر و یا دفتر مرکزی
    /// </summary>
    /// <value>
    /// CONTRACT_BANDAR
    /// </value>
    /// <value>
    /// CONTRACT_CENTER
    /// </value>
    public string Code { get; set; } = null!;

    public string Title { get; set; } = null!;

    public string? Description { get; set; }


    //Nav Property
    public ICollection<EmploymentContractTemplateVersion> Versions { get; set; } = new List<EmploymentContractTemplateVersion>();
}
