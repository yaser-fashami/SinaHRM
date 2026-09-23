using SinaHRM.Domain.Common;
using SinaHRM.Domain.Enums;

namespace SinaHRM.Domain.Entities;

public class JobLocation : BaseEntity<byte>
{
    public string JobLocationName { get; set; }
    public Port Port { get; set; }

    public ICollection<Employee> Emploies { get; set; } = [];
}
