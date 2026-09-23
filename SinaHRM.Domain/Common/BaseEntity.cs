using System.ComponentModel.DataAnnotations;

namespace SinaHRM.Domain.Common;

public abstract class BaseEntity<TKey>
{
    [Key]
    public TKey Id { get; set; }
    public DateTimeOffset CreatedAt { get; protected set; }
    public string? CreatedById { get; set; }
    public DateTimeOffset? ModifiedAt { get; protected set; }
    public string? ModifiedById { get; set; }
    public bool IsDelete { get; set; } = false;


    protected void MarkAsModified()
    {
        ModifiedAt = DateTimeOffset.UtcNow;
    }

}

