using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Linq;
using GoldenMobileX;
using Xamarin.Forms.PlatformConfiguration;
using Android;
using Android.Provider;
using Android.Content;
using Android.Widget;
using System.Text;
using Android.App;
using Android.Database;
using Android.Telephony;
using Java.Interop;
using GoldenMobileX.Models;

namespace GoldenMobileX.Views
{
    public partial class AboutPage : ContentPage
    {
        public AboutPage()
        {
            InitializeComponent();
            if (appSettings.User.ID == 0)
            {
                appSettings.GoPage(nameof(Firms));
            }

            Appearing += AboutPage_Appearing;

        }

        private void AboutPage_Appearing(object sender, EventArgs e)
        {
         
 
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {

            appSettings.GoPage(nameof(Firms));
            //await Browser.OpenAsync("http://www.goldenerp.com", BrowserLaunchMode.SystemPreferred);
        }

        private void BtnIsle_Clicked(object sender, EventArgs e)
        {
            if (EntryIslemText.Text + "" == "") return;
            if (GoldenAI.Tools.FindMoneys(EntryIslemText.Text).Count()==0) return;  
            BankaFormu bankaFormu = new BankaFormu();
            bankaFormu.viewModel = new ViewModels.FinansViewModel() { hareket = new Models.TRN_BankaHareketleri() { Aciklama = EntryIslemText.Text } };
            bankaFormu.Appearing += (s2, e2) => { bankaFormu.Isle(EntryIslemText.Text); };
            Navigation.PushAsync(bankaFormu);
        }

        private void BtnTemizle_Clicked(object sender, EventArgs e)
        {
            EntryIslemText.Text = "";

        }
        List<SMSMessage> mlist = new List<SMSMessage>();
        async void MesajlariOku()
        {

            if (!DataLayer.IsOfflineAlert)
            {
                using (GoldenContext c = new GoldenContext())
                {
                    DateTime date = (c.x_Settings.Where(s => s.SettingName == "SonSMSTarihi").Select(s => s.SettingValue)?.FirstOrDefault()).convDateTime();
                    if (date < DateTime.Now.AddDays(-3)) date = DateTime.Now.AddDays(-7);
                    List<string> list = c.AI_Dictionary.Where(s => s.Means == "BANKASMS").Select(s => s.Txt).ToList();
                    try
                    {

                        var smsService = DependencyService.Get<ISmsService>();
                        var smsMessages = smsService.GetRecentSmsMessages(date);
                        if (smsMessages.Count() == 0) { smsMessages = smsService.GetRecentSmsMessages(DateTime.Now.AddDays(-2)); }
                       
                        foreach (var m in smsMessages.Where(s => list.Contains(s.Address)).OrderByDescending(s => s.Date))
                        {
                            if (m.Date < DateTime.Now.AddDays(-3)) continue;
                            var dates = GoldenAI.Tools.FindDates(m.Body);
                            var moneys = GoldenAI.Tools.FindMoneys(m.Body);
                            if (dates.Count() > 0)
                                if (moneys.Count() > 0)
                                {
                                    if (c.TRN_BankaHareketleri.Where(s => s.Tutar == moneys.First().Item2 && s.Tarih == dates.First().Item2 && s.Aciklama.StartsWith(m.Body)).Count() > 0)
                                    {
                                        //Bu işlem daha önce girilmiş
                                        continue;
                                    }


                                    mlist.Add(new SMSMessage() { Address=m.Address, Body= m.Body, Date=m.Date});
                                }
                            var sd = c.x_Settings.Where(s => s.SettingName == "SonSMSTarihi")?.FirstOrDefault();
                            if (sd != null)
                            {
                                sd.SettingValue = m.Date + "";
                                c.x_Settings.Update(sd);
                            }
                            else
                            {
                                sd = new x_Settings() { SettingName = "SonSMSTarihi", SettingValue = m.Date + "", SettingType = "number", SettingCategory = "GENEL" };
                                c.x_Settings.Add(sd);
                            }
                            c.SaveContextWithException();
                        }
                        SMSler.ItemsSource = mlist;

                    }
                    catch (Exception ex)
                    {
                        appSettings.UyariGoster("SMS Hatası : " + ex.Message + ex.InnerException?.Message);
                    }


                }

            }
        }

        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            MesajlariOku();
            SMSStack.IsVisible = true;
            HizliIslemStack.IsVisible = false;
        }

        private void SMS_Tapped(object sender, EventArgs e)
        {
            var mi = sender as StackLayout;
            SMSMessage t = (SMSMessage)((TapGestureRecognizer)mi.GestureRecognizers.First()).CommandParameter;
            if (t != null)
            {
                if (GoldenAI.Tools.FindMoneys(t.Body).Count() == 0) return;
                BankaFormu bankaFormu = new BankaFormu();
                bankaFormu.viewModel = new ViewModels.FinansViewModel() { hareket = new Models.TRN_BankaHareketleri() { Aciklama = t.Body } };
                bankaFormu.Appearing += (s2, e2) => { bankaFormu.Isle(t.Body); };
                bankaFormu.Disappearing += (s2, e2) => { mlist.Remove(t); SMSler.ItemsSource = mlist; };
                Navigation.PushAsync(bankaFormu);
            }
        }

        private void HizliIslem_Clicked(object sender, EventArgs e)
        {
            SMSStack.IsVisible = false;
            HizliIslemStack.IsVisible = true;
        }
    }
}