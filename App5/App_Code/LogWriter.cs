using System;

/// <summary>
/// Version 1.0.0.0
/// </summary>
public class LogWriter
{

    public static string logfolder = "\\Logs";
    public static void LogYaz(string str, renk _renk)
    {
        LogYaz(str, _renk, System.DateTime.Now.ToString("dd_MM_yyyy") + ".htm");
    }

    public static void LogYaz(string str, bool lastLogs)
    {
        str = String.Format("<font color=\"#990000\" style=\"font:normal 10px arial\">{0}</font><br>", str);
        string logpath = logfolder + "\\Rapor_" + System.DateTime.Now.ToString("dd_MM_yyyy") + ".htm";

    }
    public static void LogYaz(string str, renk _renk, string filename)
    {
        try
        {
            switch (_renk)
            {
                case renk.yesil:
                    str = String.Format("<font color=\"#006600\" style=\"font:normal 10px arial\">{0}</font><br>", str);
                    break;
                case renk.sari:
                    str = String.Format("<font color=\"#A69626\" style=\"font:normal 10px arial\">{0}</font><br>", str);
                    break;
                case renk.kirmizi:
                    str = String.Format("<font color=\"#990000\" style=\"font:normal 10px arial\">{0}</font><br>", str);
                    break;
                case renk.siyah:
                    str = String.Format("<font color=\"#333333\" style=\"font:normal 10px arial\">{0}</font><br>", str);
                    break;
            }

        }
        catch { }
    }
    public enum renk
    {
        yesil, kirmizi, sari, siyah
    }

}