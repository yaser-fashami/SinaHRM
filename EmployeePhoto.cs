using SinaHRM.Domain.Common;

namespace SinaHRM.Domain.Entities;

public sealed class EmployeePhoto : BaseEntity<int>
{
    public int EmployeeId { get; private set; }

    public byte[] Data { get; private set; } = [];

    public string ContentType { get; private set; } = null!;

    public string? FileName { get; private set; }

    // Navigation Property
    public Employee Employee { get; private set; } = null!;

    private EmployeePhoto()
    {
    }

    public EmployeePhoto(
        int employeeId,
        byte[] data,
        string contentType,
        string? fileName = null)
    {
        EmployeeId = employeeId;
        Data = data;
        ContentType = contentType;
        FileName = fileName;
        CreatedAt = DateTime.UtcNow;
    }
}