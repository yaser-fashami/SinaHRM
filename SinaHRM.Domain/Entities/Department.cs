using SinaHRM.Domain.Common;

namespace SinaHRM.Domain.Entities;

public class Department : BaseEntity<byte>
{
    public string DepartmentName { get; set; }

    // Navigation Property
    public ICollection<Employee> Emploies { get; set; } = [];

}
