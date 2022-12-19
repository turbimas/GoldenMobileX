using Newtonsoft.Json;
using System;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoldenMobileX
{
    public partial class App : Application
    {

        public App()
        {

           Device.SetFlags(new string[] { "SwipeView_Experimental", "AppTheme_Experimental", "Shell_UWP_Experimental" });

            InitializeComponent();
            appSettings.User = new Models.X_Users() { ID = 0 };
            //DevExpress.XamarinForms.Editors.Initializer.Init();
            //DevExpress.XamarinForms.DataForm.Initializer.Init();
            CultureInfo customCulture = new CultureInfo("tr-TR");
            //CultureInfo customCulture = (CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();

            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("tr-TR");
            CultureInfo.DefaultThreadCurrentCulture.NumberFormat.NumberDecimalSeparator = ",";
            CultureInfo.DefaultThreadCurrentCulture.NumberFormat.NumberGroupSeparator = ".";
            CultureInfo.DefaultThreadCurrentCulture.NumberFormat.CurrencyDecimalSeparator = ",";
            customCulture.NumberFormat.NumberDecimalSeparator = ",";
            customCulture.NumberFormat.NumberGroupSeparator = ".";
            customCulture.NumberFormat.CurrencyDecimalSeparator = ",";
            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
            MainPage = new AppShell();
        }

        protected override void OnStart()
        {

        }

        protected override void OnSleep()
        {

        }

        protected override void OnResume()
        {
            if (appSettings.User.ID > 0)
            {
                appSettings.MenuYetkileri();
           
            }
            CultureInfo.CurrentCulture = new CultureInfo("tr-TR");
            CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("tr-TR");
        }
    }
}
