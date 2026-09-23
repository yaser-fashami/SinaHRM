using SinaHRM.Domain.Common;

namespace SinaHRM.Domain.Entities;

public class Profession : BaseEntity<int>
{
    public string JobName { get; set; }

    public ICollection<Employee> Emploies { get; set; } = [];

}
