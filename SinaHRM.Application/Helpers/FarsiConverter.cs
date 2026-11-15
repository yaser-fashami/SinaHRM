namespace SinaHRM.Application.Helpers;

public static class FarsiConverter
{
    internal static string ConvertMaritalStatusToFarsi(string input)
    {
        string result = string.Empty;
        switch (input)
        {
            case "single":
                result = "مجرد";
                break;
            case "married":
                result = "متاهل";
                break;
            case "divorced":
                result = "مطلقه";
                break;
            case "widowed":
                result = "بیوه";
                break;
        }
        return result;
    }

    internal static string ConvertEmploymentContractTypeToFarsi(string input)
    {
        string result = string.Empty;
        switch (input)
        {
            case "Permanent":
                result = "قرارداد دائم";
                break;
            case "FixedTerm":
                result = "قرارداد مدت معین";
                break;
            case "Temporary":
                result = "قرارداد موقت";
                break;
            case "Probationary":
                result = "قرارداد دوره آزمایشی";
                break;
        }
        return result;
    }

}
