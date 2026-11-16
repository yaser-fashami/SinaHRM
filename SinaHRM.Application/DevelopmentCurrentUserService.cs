using SinaHRM.Application.Abstractions;

namespace SinaHRM.Application;

public sealed class DevelopmentCurrentUserService
    : ICurrentUserService
{
    public Guid UserId =>
        Guid.Parse("11111111-1111-1111-1111-111111111111");
}
