using SinaHRM.Domain.Common;
using SinaHRM.Domain.Enums;

namespace SinaHRM.Domain.Entities;

public class JobLocation : BaseEntity<byte>
{
    public string JobLocationName { get; set; }
    public Branch Branch { get; set; }

    // Navigation Property
    public ICollection<Employee> Emploies { get; set; } = [];
}
