using System;
using System.ComponentModel;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

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

 

        }

        private async void Button_Clicked(object sender, EventArgs e)
        {

            appSettings.GoPage(nameof(Firms));
            //await Browser.OpenAsync("http://www.goldenerp.com", BrowserLaunchMode.SystemPreferred);
        }

 
    }
}