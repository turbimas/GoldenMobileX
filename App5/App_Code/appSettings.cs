using GoldenMobileX;
using GoldenMobileX.Models;
using GoldenMobileX.Views;
using Plugin.Media;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using Xamarin.Essentials;
using Xamarin.Forms;


public static class appSettings
{
    public static Popup activity = new Popup()
    {
        viewModel = new GoldenMobileX.ViewModels.BaseViewModel()
        {
            activityText = "Lütfen Bekleyiniz..."
        }
    };


    public static async Task UyariGoster(string txt, string baslik = "Uyarı", string cancel = "TAMAM")
    {
        await App.Current.MainPage.DisplayAlert(baslik, txt, cancel);
    }
    public static string replaceAll(string text)
    {
        return  replaceCharacters(replaceturkish(text + "").Replace("'", "").Replace(",", ""), "0,1,2,3,4,5,6,7,8,9,.,/,\\");
    }
    public static string replaceturkish(string str)
    {
        string from = "İ,ı,Ş,ş,Ç,ç,Ö,ö,Ü,ü,Ğ,ğ, ,),(";
        string to = "I,i,S,s,C,c,O,o,U,u,G,g,_,,";

        return replaceCharacters(str, from, to);
    }
    public static string replaceCharacters(string str, string fromCommaSeparatedCharacters, string toCommaSeparatedCharacters = "")
    {
        if (str == null) return str;
        int i;
        if (toCommaSeparatedCharacters == "")
        {
            for (i = 0; i <= (fromCommaSeparatedCharacters.Split(@",".ToCharArray()).Length - 1); i++)
            {
                str = str.Replace(fromCommaSeparatedCharacters.Split(@",".ToCharArray())[i], "");
            }
        }
        else
        {
            for (i = 0; i <= (fromCommaSeparatedCharacters.Split(@",".ToCharArray()).Length - 1); i++)
            {
                str = str.Replace(fromCommaSeparatedCharacters.Split(@",".ToCharArray())[i], toCommaSeparatedCharacters.Split(@",".ToCharArray())[i]);
            }
        }
        return str;
    }
    public static async Task UyariGoster(this Exception ex)
    {
        await App.Current.MainPage.DisplayAlert("UYARI", ex.Message + " " + ex.InnerException?.Message, "TAMAM");
    }
    static bool _isbusy;
    public static bool isBusy
    {
        get { return _isbusy; }
        set
        {
            _isbusy = value;

        }
    }
    public static async void GoPage(string Page)
    {
        if (Device.RuntimePlatform == Device.Android)
            await Shell.Current.GoToAsync($"//{Page}");
        else if (Device.RuntimePlatform == Device.UWP)
            await Shell.Current.GoToAsync(Page);
    }
    public static string ApiURL { get; set; }
    public static bool UseApi { get { return (ApiURL.StartsWith("http")); } }


    public static async Task<bool> Onay(string uyari = "Kayıt silinecektir onaylıyor musunuz?", string baslik = "Uyarı")
    {
        return await App.Current.MainPage.DisplayAlert(baslik, uyari, "Evet", "Hayır");

    }



    public static double ScreenWidth
    {
        get
        {
            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
            return mainDisplayInfo.Width / mainDisplayInfo.Density;
        }
    }
    public static double ScreenHeight
    {
        get
        {
            var mainDisplayInfo = DeviceDisplay.MainDisplayInfo;
            return mainDisplayInfo.Height / mainDisplayInfo.Density;
        }
    }
    public static X_Users User { get; set; }



    public static int UserDefaultFirm
    {
        get
        {
            if (appSettings.User.Firms == "")
                return -1;
            else
                try
                {
                    return conv.ToInt(appSettings.User.Firms.Split(",".ToCharArray())[0]);
                }
                catch { return -1; }
        }
    }
    public static int UserDefaultBranch
    {
        get
        {
            if (appSettings.User.Branch == "")
                return -1;
            else
                try
                {
                    return conv.ToInt(appSettings.User.Branch.Split(",".ToCharArray())[0]);
                }
                catch { return -1; }
        }
    }
    public static string CurrentFirm
    {
        get;
        set;
    }

    public static DateTime CalismaTarihi1
    {
        get;
        set;
    }

    public static DateTime CalismaTarihi2
    {
        get;
        set;
    }
    public class Photo
    {
        public EventHandler PhotoTaken;
        public Plugin.Media.Abstractions.MediaFile PhotoFile;
        public async Task PhotoTake()
        {

            var PhotoFile = await CrossMedia.Current.TakePhotoAsync(new Plugin.Media.Abstractions.StoreCameraMediaOptions
            {
                Directory = "GoldenERPPictures",
                DefaultCamera = Plugin.Media.Abstractions.CameraDevice.Rear,
                Name = DateTime.Now.ToString("yyyyMMddHHmmss") + ".jpg"

            });



            if (PhotoFile != null)
                if (PhotoTaken != null)
                    PhotoTaken(null, null);
        }
    }

    public static string GetCounter(string Module, string TableName, string ColumnName, string startmask = "", bool CheckDublicateFiche = true)
    {

        return Module.Substring(0, 1) + System.DateTime.Now.ToString("ddMMyyHHmmss");
    }
    public static void GetExchanges(DateTime date, DateTime kurTarihi)
    {

        foreach (DataRow dr in TurbimSQLHelper.defaultconn.SQLSelectToDataTable("SELECT CurrencyNumber, CurrencyCode FROM X_Currency").Rows)
        {
            if (TurbimSQLHelper.defaultconn.SQLSelect("SELECT COUNT(ID) FROM TRN_DailyExchange WHERE CurrencyID=" + dr["CurrencyNumber"].convInt() + " AND [Date]='" + date.ToString("yyyy-MM-dd") + "'") == "0")
            {
                string[] curr = KurHesapla(1, dr["CurrencyCode"] + "", kurTarihi);
                if (curr[0] != null)
                {
                    if (dr["CurrencyCode"] + "" != "TL")
                        TurbimSQLHelper.defaultconn.SQLExecuteNonQuery("INSERT INTO TRN_DailyExchange(Date, CurrencyID, RATE1, RATE2, RATE3, RATE4) VALUES('" + date.ToString("yyyy-MM-dd") + "'," + dr["CurrencyNumber"] + ",'" + curr[0] + "','" + curr[1] + "','" + curr[2] + "','" + curr[3] + "')");
                }
                else
                {
                    if (date < DateTime.Now.AddDays(1))
                        GetExchanges(date, kurTarihi.AddDays(-1));
                }
            }
        }

    }

    public static double KurCevir(double tutar, int Kod, DateTime date, int KurTipNo)
    {
        if (date < System.DateTime.Now.AddYears(-100))
            return 1;
        if (KurTipNo == 0)
            KurTipNo = 2;
        if (Kod == 0)
            return 1;
        double kur = 1;
        DataRow dr = TurbimSQLHelper.defaultconn.SQLSelectDataRow("SELECT RATE" + KurTipNo + " FROM TRN_DailyExchange WHERE CurrencyID=" + Kod + " AND  [Date]='" + date.ToString("yyyy-MM-dd") + "'");
        if (dr == null)
        {
            GetExchanges(date, date);
            kur = conv.ToDouble(TurbimSQLHelper.defaultconn.SQLSelect("SELECT RATE" + KurTipNo + " FROM TRN_DailyExchange WHERE CurrencyID=" + Kod + " AND   [Date]='" + date.ToString("yyyy-MM-dd") + "'"));
        }
        else
            kur = conv.ToDouble(dr[0]);
        if (kur == 0)
        {
            kur = 1;
        }
        return kur * tutar;
    }
    static string url = "";
    static string xmlstring = "";
    static string[] KurHesapla(double tutar, string kod, DateTime date)
    {
        if (kod == "0" || kod == "TL")
            return new string[] { "1", "1", "1", "1" };


        double[] _tl = new double[4];
        string[] tlstring = new string[4];
        try
        {
            if (conv.ToInt(date.DayOfWeek) == 0)
            { date = date.AddDays(-2); }
            else if (conv.ToInt(date.DayOfWeek) == 1)
            { date = date.AddDays(-3); }
            else
            {
                date = date.AddDays(-1);
            }
            if (url != "http://www.tcmb.gov.tr/kurlar/" + date.ToString("yyyyMM") + "/" + date.ToString("ddMMyyyy") + ".xml" || !xmlstring.Contains("<?xml"))
            {
                url = "http://www.tcmb.gov.tr/kurlar/" + date.ToString("yyyyMM") + "/" + date.ToString("ddMMyyyy") + ".xml";
                xmlstring = TurbimTools.GetRemotePage(url);
                if (!xmlstring.Contains("<?xml"))
                {
                    xmlstring = TurbimTools.GetRemotePage(url, TurbimTools.getinnertext(xmlstring, "document.cookie=\"", "\";", true).Replace("\"", ""));
                }
                if (!xmlstring.Contains("<?xml")) return new string[] { "1", "1", "1", "1" };
            }
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(xmlstring);

            System.Xml.XmlNodeList xnList = xmlDocument.ChildNodes[2].SelectNodes("Currency[@Kod='" + kod + "']");
            _tl[0] = tutar * (Convert.ToDouble(xnList[0].ChildNodes[3].InnerText.Replace(".", ",")) / Convert.ToDouble((xnList[0].ChildNodes[0].InnerText.Replace(".", ","))));
            _tl[1] = tutar * (Convert.ToDouble(xnList[0].ChildNodes[4].InnerText.Replace(".", ",")) / Convert.ToDouble((xnList[0].ChildNodes[0].InnerText.Replace(".", ","))));
            _tl[2] = tutar * (Convert.ToDouble(xnList[0].ChildNodes[5].InnerText.Replace(".", ",")) / Convert.ToDouble((xnList[0].ChildNodes[0].InnerText.Replace(".", ","))));
            _tl[3] = tutar * (Convert.ToDouble(xnList[0].ChildNodes[6].InnerText.Replace(".", ",")) / Convert.ToDouble((xnList[0].ChildNodes[0].InnerText.Replace(".", ","))));
            tlstring[0] = (_tl[0] + "").Replace(",", ".");
            tlstring[1] = (_tl[1] + "").Replace(",", ".");
            tlstring[2] = (_tl[2] + "").Replace(",", ".");
            tlstring[3] = (_tl[3] + "").Replace(",", ".");

        }
        catch (Exception ex)
        {
            var st = new System.Diagnostics.StackTrace(ex, true);

            var frame = st.GetFrame(0);
            // Get the line number from the stack frame
            var line = frame.GetFileLineNumber();
            LogWriter.LogYaz("Kurlar belirtilmemiş. Lütfen manuel olarak günlük döviz kurlarını giriniz." + frame.GetFileName() + " " + line + " - " + frame.GetFileColumnNumber(), LogWriter.renk.kirmizi);
        }
        return tlstring;

    }
    public static int UserDepartman
    {
        get;
        set;
    }
    public static string UserPhone
    {
        get;
        set;
    }
    public static string UserMail
    {
        get;
        set;
    }
    public static string UserGorevi
    {
        get;
        set;
    }
    public static string UserAuthCode
    {
        get;
        set;
    }
    public static int UpperUserID
    {
        get;
        set;
    }
    static string IDler = "";


    public static localSettings LocalSettings { get; set; }
    public static offlineData OfflineData { get; set; }
    public class localSettings
    {
        public localSettings()
        {
            Firms = new List<X_Firms>();
            UserSettings = new List<X_UserSettings>();
        }
        List<X_Firms> _Firms;
        public List<X_Firms> Firms { get { if (_Firms == null) _Firms = new List<X_Firms>(); return _Firms; } set { _Firms = value; } }

        static List<X_UserSettings> _UserSettings;
        public List<X_UserSettings> UserSettings { get { if (_UserSettings == null) _UserSettings = new List<X_UserSettings>(); return _UserSettings; } set { _UserSettings = value; } }

        bool _onlineAktarim = false;

        public bool OnlineOluncaOtomatikAktarim
        {
            get { return _onlineAktarim; }
            set
            {
                _onlineAktarim = value;
                if (appSettings.User != null)
                    if (appSettings.User.ID > 0)
                        if (DataLayer.IsOnline)
                            if (_onlineAktarim)
                                Device.StartTimer(new TimeSpan(0, 3, 0), () =>
                                {
                                    Device.BeginInvokeOnMainThread(() =>
                                    {
                                        if (DataLayer.WaitingSent.tRN_StockTrans.Count() > 0)
                                        {
                                            for (int i = 1; i <= DataLayer.WaitingSent.tRN_StockTrans.Count(); i++)
                                                DataLayer.TRN_StockTransInsert(DataLayer.WaitingSent.tRN_StockTrans.Take(1).First(), false);
                                        }
                                        if (DataLayer.WaitingSent.tRN_Orders.Count() > 0)
                                        {
                                            for (int i = 1; i <= DataLayer.WaitingSent.tRN_Orders.Count(); i++)
                                                DataLayer.TRN_OrdersInsert(DataLayer.WaitingSent.tRN_Orders.Take(1).First(), false);
                                        }
                                        if (DataLayer.WaitingSent.TRN_EtiketBasim.Count() > 0)
                                        {
                                            for (int i = 1; i <= DataLayer.WaitingSent.TRN_EtiketBasim.Count(); i++)
                                                DataLayer.TRN_EtiketBasimInsert(DataLayer.WaitingSent.TRN_EtiketBasim.Take(1).First(), false);
                                        }
                                    });
                                    return value; // runs again, or false to stop
                                });
            }
        }
    }
    public class offlineData
    {
        public offlineData()
        {
            SQLListToRun = new List<string>();
            JSON_TRN_StockTransListToRun = new List<string>();
            TRN_StockTransLines = new List<TRN_StockTransLines>();
        }
        public List<string> SQLListToRun
        {
            get; set;
        }


        public List<string> JSON_TRN_StockTransListToRun
        {
            get; set;
        }

        public List<TRN_StockTransLines> TRN_StockTransLines
        { get; set; }
    }

    public static void MenuYetkileri()
    {

        if (appSettings.User.LogonName.ToLower() != "admin")
        {
            if (appSettings.UseApi)
            {
            }
            else
            {
                string MenuSql = @"SELECT   M.ID, M.FormName, M.VisibleName, M.GroupID, M.MenuID, ISNULL(M.[Image], M.FormName) AS Image FROM X_Modules M 
WHERE  (ISNULL(M.UpperFormID,0)=0 AND M.VisibleName NOT LIKE '%*') AND 
M.ID IN (SELECT R.ControlID FROM V_UserRights R WHERE  R.UserID=@UserID AND R.Visible=1 AND R.IsMenuItem=1) AND M.FormName IS NOT NULL Order BY M.Sira, M.ID ".Replace("@UserID", appSettings.User.ID + "");
                System.Data.DataTable dt = db.SQLSelectToDataTable(MenuSql);
                if (dt.AsEnumerable().Where(s => s["FormName"] + "" == "Stoklar" || s["FormName"] + "" == "Malzemeler").Count() == 0) (Shell.Current as AppShell).flyoutItemStoklar.IsVisible = false;
                if (dt.AsEnumerable().Where(s => s["FormName"] + "" == "SatisPazarlama").Count() == 0) (Shell.Current as AppShell).flyoutItemSatisPazarlama.IsEnabled = false;
                if (dt.AsEnumerable().Where(s => s["FormName"] + "" == "SatinAlma").Count() == 0) (Shell.Current as AppShell).flyoutItemSatinAlma.IsEnabled = false;
                if (dt.AsEnumerable().Where(s => s["FormName"] + "" == "Finans").Count() == 0) (Shell.Current as AppShell).flyoutItemFinans.IsVisible = false;
            }
        }
    }




    public static List<TRN_StockTrans> StockTrans { get; set; }
    public static List<TRN_Invoice> Invoices { get; set; }

    public static void DeleteUserData()
    {
        try
        {
            App.Current.MainPage.IsBusy = true;
            appSettings.LocalSettings.Firms = new List<X_Firms>();
            appSettings.LocalSettings.UserSettings = new List<X_UserSettings>();
            appSettings.LocalSettings.SaveXML();

            appSettings.OfflineData.SQLListToRun = new List<string>();
            appSettings.OfflineData.TRN_StockTransLines = new List<TRN_StockTransLines>();
            appSettings.OfflineData.JSON_TRN_StockTransListToRun = new List<string>();
            appSettings.OfflineData.SaveXML();
            App.Current.MainPage.IsBusy = false;
        }
        catch (Exception ex)
        {
            appSettings.UyariGoster(ex.Message);
            App.Current.MainPage.IsBusy = false;
        }
    }



}

