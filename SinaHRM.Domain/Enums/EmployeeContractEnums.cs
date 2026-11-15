namespace SinaHRM.Domain.Enums;

public enum EmploymentContractStatus : byte
{
    /// <summary>
    /// پیش نویس
    /// </summary>
    Draft = 1,
    /// <summary>
    /// در انتظار امضاء
    /// </summary>
    PendingSignature = 2,
    /// <summary>
    /// امضا شده
    /// </summary>
    Signed = 3,
    /// <summary>
    /// فعال
    /// </summary>
    Active = 4,
    /// <summary>
    /// منقضی شده
    /// </summary>
    Expired = 5,
    /// <summary>
    /// لغو شده
    /// </summary>
    Cancelled = 6,
    /// <summary>
    /// برگشت داده شده
    /// </summary>
    Rejected = 7
}

public enum EmploymentContractType : byte
{
    /// <summary>
    /// قرارداد دائم
    /// </summary>
    Permanent = 1,
    /// <summary>
    /// قرارداد مدت معین
    /// </summary>
    FixedTerm = 2,
    /// <summary>
    /// قرارداد موقت
    /// </summary>
    Temporary = 3,
    /// <summary>
    /// قرارداد دوره آزمایشی
    /// </summary>
    Probationary = 4
}

public enum FinancialItemType : byte
{
    /// <summary>
    /// درآمدی
    /// </summary>
    Earning = 1,
    /// <summary>
    /// کسر شونده
    /// </summary>
    Deduction = 2
}