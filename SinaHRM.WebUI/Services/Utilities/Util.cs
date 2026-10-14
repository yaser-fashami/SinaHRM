namespace SinaHRM.WebUI.Services.Utilities;

public static class Util
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
}
