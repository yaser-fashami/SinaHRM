using SinaHRM.Domain.Common;

namespace SinaHRM.Domain.Entities;

public class Membership : BaseEntity<byte>
{
    public string CompanyName { get; set; }
    public string? ApearanceCompanyName { get; set; }
    public string? TitleCompany{ get; set; }
    public string? AcountCode{ get; set; }
    public string? CEO { get; set; }
    public string? Address { get; set; }
    public string? PostalCode { get; set; }


    public ICollection<Employee> Emploies { get; set; } = [];

}
