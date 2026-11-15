using System.ComponentModel.DataAnnotations;

namespace SinaHRM.Domain.Common;

public abstract class BaseEntity<TKey>
{
    [Key]
    public TKey Id { get; set; }
    public DateTimeOffset CreatedAt { get; protected set; }
    [MaxLength(450)]
    public string? createdById { get; set; }
    public DateTimeOffset? ModifiedAt { get; protected set; }
    [MaxLength(450)]
    public string? ModifiedById { get; set; }
    public bool IsDelete { get; set; } = false;


    protected void MarkAsModified()
    {
        ModifiedAt = DateTimeOffset.UtcNow;
    }

}

