using GoldenMobileX.ViewModels;
using GoldenMobileX.Views;
using System;
using System.Collections.Generic;
using System.Data;
using Xamarin.Forms;
using System.Linq;
using GoldenMobileX.Models;

namespace GoldenMobileX
{
    public partial class AppShell : Xamarin.Forms.Shell
    {
        public FlyoutItem flyoutItemFinans, flyoutItemSatinAlma, flyoutItemSatisPazarlama, flyoutItemStoklar;

        private void Sevkiyat_Clicked(object sender, EventArgs e)
        {
            appSettings.GoPage($"{nameof(StokFisleri)}?Type=5");
        }

        public AppShell()
        {
            InitializeComponent();
            if (Device.RuntimePlatform == Device.UWP)
            {
                 Routing.RegisterRoute(nameof(Firms), typeof(Firms));
                Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
                Routing.RegisterRoute(nameof(AboutPage), typeof(AboutPage));
            }

            appSettings.OfflineData = new Models.offlineData();
            appSettings.LocalSettings = new Models.localSettings();

            flyoutItemFinans = FlyoutItemFinans;
            flyoutItemSatinAlma = FlyoutItemSatinAlma;
            flyoutItemSatisPazarlama = FlyoutItemSatisPazarlama;
            flyoutItemStoklar = FlyoutItemStoklar;
            Appearing += AppShell_Appearing;
        }

        private void AppShell_Appearing(object sender, EventArgs e)
        {
      
        }

        private async void OnMenuItemClicked(object sender, EventArgs e)
        {
            appSettings.User.ID = 0;
            (Application.Current).MainPage = new AppShell();
            appSettings.GoPage(nameof(LoginPage));
        }
    }
}
