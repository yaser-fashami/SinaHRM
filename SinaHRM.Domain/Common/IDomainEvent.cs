namespace SinaHRM.Domain.Common;

public interface IDomainEvent
{
    DateTimeOffset OccurredOn { get; }

}
