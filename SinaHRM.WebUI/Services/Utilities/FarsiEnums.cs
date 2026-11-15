namespace SinaHRM.WebUI.Services.Utilities;

public static class FarsiEnums
{
    internal static string ConvertEducationalQualificationToFarsi(string input)
    {
        string result = string.Empty;
        switch (input)
        {
            case "underDiploma": result ="زیردیپلم";
                    break;
            case "diploma": result = "دیپلم";
                    break;
            case "postDiploma": result = "فوق دیپلم";
                    break;
            case "bachelor": result = "لیسانس";
                    break;
            case "master": result = "فوق لیسانس";
                    break;
            case "doctor": result = "دکتری";
                    break;
            case "postDoctor": result = "فوق دکتری";
                    break;
            case "hozavi": result = "حوزوی";
                    break;
            case "Unknow": result = "نامشخص";
                    break;
        }
        return result;
    }
    internal static string ConvertContractStatusToFarsi(string input)
    {
        string result = string.Empty;
        switch (input)
        {
            case "Draft": result = " پیش نویس";
                    break;
            case "PendingSignature": result = "در انتظار امضاء";
                    break;
            case "Signed": result = "امضا شده";
                    break;
            case "Active": result = "فعال";
                    break;
            case "Expired": result = "منقضی شده";
                    break;
            case "Cancelled": result = "لغو شده";
                    break;
            case "Rejected": result = "برگشت داده شده";
                    break;
        }
        return result;
    }
}
