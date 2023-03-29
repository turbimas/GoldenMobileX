using GoldenMobileX.Models;
using System.Collections.Generic;
using System.Data;
using System.Linq;

class appParams
{
    public class Setting
    {
        public static string Set(string SettingName, string SettingValue, string SettingDesc = null)
        {
            SettingName = SettingName.Replace("get_", "").Replace("set_", "");
            List<x_Settings> drs = DataLayer.X_Settings.Where(x => x.SettingName == SettingName).ToList();

            if (drs.Count == 0)
            {
                DataLayer.X_Settings.Add(new x_Settings() { SettingName = SettingName, SettingValue = SettingValue, SettingDesc = (SettingDesc != null ? SettingDesc : SettingName) });

            }
            else
            {
                drs.First().SettingValue = SettingValue;

            }
            // appSettings.DATA.SettingsTA.Update(appSettings.DATA.SettingsDT);
            return SettingValue;
        }
        public static string lastSettingDesc = "";
        public static string Get(string SettingName, string defaultValue = "", string desc = "", string Category = "GENEL", string type = "text", string settingCoices = "")
        {
            SettingName = SettingName.Replace("get_", "").Replace("set_", "");
            List<x_Settings> drs = DataLayer.X_Settings.Where(x => x.SettingName == SettingName).ToList();

            if (drs.Count == 0)
            {
                Set(SettingName, defaultValue, desc);
            }
            if (drs.Count == 0) return "";
            else
                return drs.First().SettingValue;
        }

    }

    public class Genel
    {
        static string Module = "GENEL";
        public static bool DosyalarSunucuDiskineKaydedilsin
        {
            get { return Setting.Get("DosyalarSunucuDiskineKaydedilsin", "FALSE", "Dosyalar sunucu diskine kaydedilsin", Module, "check").convBool(); }
        }
        public static bool KullanicilarTanimliTerminalleriKullansin
        {
            get { return Setting.Get("KullanicilarTanimliTerminalleriKullansin", "FALSE", "Kullanıcılar sadece yetkili oldukları bilgisayarlarda oturum açabilsinler.", Module, "check").convBool(); }
        }
        public static bool EncrypteConnection
        {
            get { return Setting.Get("EncrypteConnection", "FALSE", "Sql Bağlantısını şifrele", Module, "check").convBool(); }
        }
        public static string SunucuDosyaKayitYolu
        {
            get { return Setting.Get("SunucuDosyaKayitYolu", "c:\\GoldenFiles", "Sunucuda dosyaların ve evrakların kaydedileceği yol", Module); }
        }
        public static string SunucuIPAdresi
        {
            get { return Setting.Get("SunucuIPAdresi", "", "Haberleşme için sunucunun IP adresini giriniz. Socket bağlantıları için gereklidir.", Module); }
        }
        public static int SunucuPortu
        {
            get { return Setting.Get("SunucuPortu", "2000", "Haberleşme için sunucunun port numarasını giriniz. Socket bağlantıları için gereklidir.", Module).convInt(); }
        }
        public static string SQLYedekKlasorYolu
        {
            get { return Setting.Get("SQLYedekKlasorYolu", "c:\\GoldenFiles", "SQL yedeklerinin tutulacağı yol", Module); }
        }
        public static bool UseEnterAsTab
        {
            get { return Setting.Get("UseEnterAsTab", "FALSE", "ENTER tuşunu TAB tuşu gibi kullan", Module, "check").convBool(); }
        }
        public static int YeniMalzemeFislerindeDurum
        {
            get { return Setting.Get("YeniMalzemeFislerindeDurum", "4", "Terminalden yapılan yeni malzeme fişlerinde varsayılan durum kodu.", "STOKLAR").convInt(); }
        }
    }


    public static class UserSettings
    {
        public static bool BeniHatirla
        {
            get { return checkUserSettings(System.Reflection.MethodBase.GetCurrentMethod().Name, false.ToString()).convBool(); }
            set { setUserSettings(System.Reflection.MethodBase.GetCurrentMethod().Name, value + ""); }
        }
        public static string UserName
        {
            get { return checkUserSettings(System.Reflection.MethodBase.GetCurrentMethod().Name, false.ToString()) + ""; }
            set { setUserSettings(System.Reflection.MethodBase.GetCurrentMethod().Name, value + ""); }
        }
        public static string DefaultConnString
        {
            get { return checkUserSettings(System.Reflection.MethodBase.GetCurrentMethod().Name, false.ToString()) + ""; }
            set { setUserSettings(System.Reflection.MethodBase.GetCurrentMethod().Name, value + ""); }
        }
        public static string Password
        {
            get { return checkUserSettings(System.Reflection.MethodBase.GetCurrentMethod().Name, false.ToString()) + ""; }
            set { setUserSettings(System.Reflection.MethodBase.GetCurrentMethod().Name, value + ""); }
        }
        public static string setUserSettings(string Name, string defaultValue)
        {
            Name = Name.Replace("get_", "").Replace("set_", "");
            if (appSettings.LocalSettings.UserSettings.Where(x => x.SettingName == Name).Count() == 0)
            {
                var maxID = 1;
                var m = appSettings.LocalSettings.UserSettings.OrderByDescending(x => x.ID).FirstOrDefault();
                if (m != null)
                    maxID = m.ID + 1;
                X_UserSettings s = new X_UserSettings() { SettingName = Name, SettingValue = defaultValue, ID = maxID };
                appSettings.LocalSettings.UserSettings.Add(s);
                appSettings.LocalSettings.SaveXML();

            }
            else
            {
                X_UserSettings s = appSettings.LocalSettings.UserSettings.Where(x => x.SettingName == Name).FirstOrDefault();
                s.SettingValue = defaultValue;
                appSettings.LocalSettings.SaveXML();
            }
            return appSettings.LocalSettings.UserSettings.Where(x => x.SettingName == Name).FirstOrDefault().SettingValue + "";
        }
        public static string checkUserSettings(string Name, string defaultValue)
        {
            Name = Name.Replace("get_", "").Replace("set_", "");
            if (appSettings.LocalSettings.UserSettings.Where(x => x.SettingName == Name).Count() == 0)
            {
                var maxID = 1;
                var m = appSettings.LocalSettings.UserSettings.OrderByDescending(x => x.ID).FirstOrDefault();
                if (m != null)
                    maxID = m.ID + 1;
                X_UserSettings s = new X_UserSettings() { SettingName = Name, SettingValue = defaultValue, ID = maxID };
                appSettings.LocalSettings.UserSettings.Add(s);
                appSettings.LocalSettings.SaveXML();
            }
            return appSettings.LocalSettings.UserSettings.Where(x => x.SettingName == Name).FirstOrDefault().SettingValue + "";
        }
    }
}
