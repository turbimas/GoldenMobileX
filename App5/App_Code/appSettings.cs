using GoldenMobileX;
using GoldenMobileX.Models;
using GoldenMobileX.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using Xamarin.Forms;
using ZXing.Net.Mobile.Forms;
using System.Linq;
using System.IO;
using System.Data;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using Plugin.Media;
using Xamarin.Essentials;
using System.Diagnostics;
using System.Collections.Concurrent;

public class appSettings
{
    static ActivityIndicator activityIndicator = new ActivityIndicator { IsRunning = false, Color = Color.Blue };
    public static void UyariGoster(string txt, string baslik = "Uyarı", string cancel = "TAMAM")
    {
        App.Current.MainPage.DisplayAlert(baslik, txt, cancel);
    }
    static bool _isbusy;
   public static bool isBusy
    {
        get { return _isbusy; }
        set
        {
            _isbusy = value;
            activityIndicator.IsRunning = value;
          
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
 
        return Module.Substring(0,1)+ System.DateTime.Now.ToString("ddMMyyHHmmss");
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


 
 
    public static localSettings LocalSettings
    {
        get;set;
    }
    public static offlineData OfflineData
    {
        get; set;
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
                if (dt.AsEnumerable().Where(s => s["FormName"] + "" == "SatisPazarlama").Count() == 0) (Shell.Current as AppShell).flyoutItemSatisPazarlama.IsVisible = false;
                if (dt.AsEnumerable().Where(s => s["FormName"] + "" == "SatinAlma").Count() == 0) (Shell.Current as AppShell).flyoutItemSatinAlma.IsVisible = false;
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

