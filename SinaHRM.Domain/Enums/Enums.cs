namespace SinaHRM.Domain.Enums;

public enum EducationalQualification
{
    /// <summary>
    /// زیردیپلم
    /// </summary>
    underDiploma = 0,
    /// <summary>
    /// دیپلم
    /// </summary>
    diploma = 1,
    /// <summary>
    /// فوق دیپلم
    /// </summary>
    postDiploma = 2,
    /// <summary>
    /// لیسانس
    /// </summary>
    bachelor = 3,
    /// <summary>
    /// فوق لیسانس
    /// </summary>
    master = 4,
    /// <summary>
    /// دکتری
    /// </summary>
    doctor = 5,
    /// <summary>
    /// فوق دکتری
    /// </summary>
    postDoctor = 6,
}

public enum Gender
{
    female = 0,
    male = 1,
}

public enum Port
{
    headquarter = 0,
    rajaee = 1,
    boushehr = 2,
    chabahar = 3,
    emam = 4,
    anzali = 5,
    amirabad = 6,
    noshahr = 7,
}

public enum ManagementLayer
{
    layer1 = 1,
    layer2 = 2,
    layer3 = 3,
    layer4 = 4,
    layer5 = 5,
    layer6 = 6,
}

public enum MaritalStatus
{
    /// <summary>
    /// مجرد
    /// </summary>
    single = 0,
    /// <summary>
    /// متاهل
    /// </summary>
    married = 1,
    /// <summary>
    /// مطلقه
    /// </summary>
    divorced = 2,
    /// <summary>
    /// بیوه
    /// </summary>
    widowed = 3,
}