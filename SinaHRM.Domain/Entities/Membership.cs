using SinaHRM.Domain.Common;

namespace SinaHRM.Domain.Entities;

public class Membership : BaseEntity<byte>
{
    public string CompanyName { get; set; }
    public string? ApearanceCompanyName { get; set; }
    public string? TitleCompany{ get; set; }
    public string? AcountCode{ get; set; }
    public string? CEO { get; set; }
}
