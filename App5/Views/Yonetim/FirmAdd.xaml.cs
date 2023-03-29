using GoldenMobileX.Models;
using System;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoldenMobileX.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FirmAdd : ContentPage
    {
        public FirmAdd()
        {
            InitializeComponent();
        }
        private void ButtonKaydet_Clicked(object sender, EventArgs e)
        {

            if (EntryName.Text + "" == "")
            {
                appSettings.UyariGoster("Lütfen firma adını yazınız...");
                EntryName.Focus();
                return;
            }
            if (EntrySunucu.Text + "" == "")
            {
                appSettings.UyariGoster("Lütfen sunucu adını yazınız...");
                EntrySunucu.Focus();
                return;
            }
            if (EntrySunucu.Text.ToLower().StartsWith("http"))
            {
                int firmNR = 1;
                if (appSettings.LocalSettings.Firms.Count > 0)
                {
                    firmNR = appSettings.LocalSettings.Firms.Max(x => x.FirmNr.convInt()) + 1;
                }
                appSettings.LocalSettings.Firms.Add(new X_Firms() { FirmNr = firmNR, Server = EntrySunucu.Text, DataBase = EntryVeriTabani.Text, Name = EntryName.Text, User = EntryKullanici.Text, Pass = EntrySifre.Text });
                if (appSettings.LocalSettings.Firms.Count > 0)
                {
                    appSettings.LocalSettings.SaveXML();
                    appSettings.UyariGoster("Sunucu ayarları doğru. Firma Eklendi..");
                    Navigation.PopAsync();
                }
                else appSettings.UyariGoster("Hata oluştu..");

            }
            else
            {
                TurbimSQLHelper t = new TurbimSQLHelper(string.Format("Data Source={0};Initial Catalog={1};User ID={2};Password={3};Connection Timeout=3", EntrySunucu.Text, EntryVeriTabani.Text, EntryKullanici.Text, EntrySifre.Text));
                if (t.isOpen())
                {
                    int firmNR = 1;
                    if (appSettings.LocalSettings.Firms.Count > 0)
                    {
                        firmNR = appSettings.LocalSettings.Firms.Max(x => x.FirmNr.convInt()) + 1;
                    }
                    appSettings.LocalSettings.Firms.Add(new X_Firms() { FirmNr = firmNR, Server = EntrySunucu.Text, DataBase = EntryVeriTabani.Text, Name = EntryName.Text, User = EntryKullanici.Text, Pass = EntrySifre.Text });
                    if (appSettings.LocalSettings.Firms.Count > 0)
                    {
                        appSettings.LocalSettings.SaveXML();
                        appSettings.UyariGoster("Sunucu ayarları doğru. Firma Eklendi..");
                        Navigation.PopAsync();
                    }
                    else appSettings.UyariGoster("Hata oluştu..");
                }
                else
                {
                    appSettings.UyariGoster("Lütfen bağlantı ayarlarını kontrol ediniz.");
                }
            }
        }

        private void IsApi_Toggled(object sender, ToggledEventArgs e)
        {
            bool isapi = (sender as Switch).IsToggled;
            if (isapi)
            {
                UserInfo.IsVisible = false;
                EntryVeriTabani.IsVisible = false;

            }
            else
            {
                UserInfo.IsVisible = true;
                EntryVeriTabani.IsVisible = false;
            }
        }
    }
}