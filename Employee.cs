using SinaHRM.Domain.Common;
using SinaHRM.Domain.Enums;

namespace SinaHRM.Domain.Entities;

public class Employee : BaseEntity<int>
{
    /// <summary>
    /// کد پرسنلی
    /// </summary>
    public string PersonnelCode { get; set; }
    /// <summary>
    /// نام
    /// </summary>
    public string FirstName { get; set; }
    /// <summary>
    /// نام خانوادگی
    /// </summary>
    public string LastName { get; set; }
    /// <summary>
    /// سن
    /// </summary>
    public byte? Age { get; set; }
    /// <summary>
    /// تاریخ تولد
    /// </summary>
    public DateTime? BirthDate { get; set; }
    /// <summary>
    /// نام پدر
    /// </summary>
    public string FatherName { get; set; }
    /// <summary>
    /// کد ملی
    /// </summary>
    public string NationalCode { get; set; }
    /// <summary>
    /// شماره شناسنامه
    /// </summary>
    public string BirthCertificateNumber { get; set; }
    /// <summary>
    /// محل تولد
    /// </summary>
    public string PlaceBirth { get; set; }
    /// <summary>
    /// محل صدور شناسنامه
    /// </summary>
    public string PlaceOfIssuanceOfBirthCertificate { get; set; }
    /// <summary>
    /// جنسیت
    /// </summary>
    public Gender Gneder { get; set; }
    /// <summary>
    /// مدرک تحصیلی
    /// </summary>
    public EducationalQualification EducationalQualification { get; set; }
    /// <summary>
    /// شماره همراه
    /// </summary>
    public string? MobileNumber { get; set; }
    /// <summary>
    /// شماره ثابت
    /// </summary>
    public string? PhoneNumber { get; set; }
    /// <summary>
    /// ایمیل
    /// </summary>
    public string? Email { get; set; }
    /// <summary>
    /// آدرس پستی
    /// </summary>
    public string? Address { get; set; }
    /// <summary>
    /// کد پستی
    /// </summary>
    public string? PostalCode { get; set; }
    /// <summary>
    /// Id عضویت
    /// </summary>
    public byte MembershipId { get; set; }
    /// <summary>
    /// عضویت
    /// </summary>
    public Membership Membership { get; set; }
    /// <summary>
    /// Id شغل
    /// </summary>
    public int ProfessionId { get; set; }
    /// <summary>
    /// شغل
    /// </summary>
    public Profession Profession { get; set; }
    /// <summary>
    /// id محل خدمت
    /// </summary>
    public byte JobLocationId { get; set; }
    /// <summary>
    ///  محل خدمت
    /// </summary>
    public JobLocation JobLocation { get; set; }
    /// <summary>
    /// واحدسازمانیId
    /// </summary>
    public byte? DepartmentId { get; set; }
    /// <summary>
    /// واحدسازمانی
    /// </summary>
    public Department Department { get; set; }
    /// <summary>
    /// لایه مدیریتی
    /// </summary>
    public ManagementLayer? ManagementLayer { get; set; }
    /// <summary>
    /// وضعیت تاهل
    /// </summary>
    public MaritalStatus MaritalStatus { get; set; }
    /// <summary>
    /// تعداد فرزند
    /// </summary>
    public byte? NumberOfChildren { get; set; }


    /// <summary>
    /// حقوق مبنا
    /// </summary>
    public decimal BaseSalary { get; set; }
    /// <summary>
    /// حق مسئولیت
    /// </summary>
    public decimal? ResponsibilitySalary { get; set; }
    /// <summary>
    /// پایه سنوات
    /// </summary>
    public decimal? YearsBase { get; set; }
    /// <summary>
    /// حق مسکن
    /// </summary>
    public decimal? HousingBonous { get; set; }
    /// <summary>
    /// اقلام مصرفی خانوار-بن کارگری
    /// </summary>
    public decimal? WorkerBonous { get; set; }
    /// <summary>
    /// حق تاهل
    /// </summary>
    public decimal? MartialBonous { get; set; }
    /// <summary>
    /// حق اولاد
    /// </summary>
    public decimal? ChildrenBonous { get; set; }
    /// <summary>
    /// معیل - فردی که شخص یا اشخاص دیگری تحت تکفل مالی او هستند
    /// </summary>
    public bool? HasDependents { get; set; }



    /// <summary>
    /// مجموع حکم
    /// </summary>
    public decimal TotalSalary => BaseSalary + ResponsibilitySalary.Value + YearsBase.Value + HousingBonous.Value + WorkerBonous.Value + MartialBonous.Value + ChildrenBonous.Value;
    /// <summary>
    /// نام کامل
    /// </summary>
    public string FullName => FirstName + " " + LastName;
    /// <summary>
    /// مرکز هزینه
    /// </summary>
    public string? CostCenter => Membership.ApearanceCompanyName + '-' + JobLocation.Branch.ToString();

    // Navigation Property
    public ICollection<EmployeePhoto>? Photos { get; set; } = [];

}
