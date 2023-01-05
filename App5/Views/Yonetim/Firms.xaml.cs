using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoldenMobileX.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Collections.ObjectModel;
using GoldenMobileX.Models;
using System.Diagnostics;
using System.Data;
using System.ComponentModel;



namespace GoldenMobileX.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Firms : ContentPage
    {
        FirmsViewModel viewModel
        {
            get { return this.BindingContext as FirmsViewModel; }
        }
        public Firms()
        {
            InitializeComponent();
            BindingContext = new FirmsViewModel();
            Appearing += Firms_Appearing;
            Rebind();
     
        }

 
        private void Firms_Appearing(object sender, EventArgs e)
        {

            //VersiyonLabel.Text = Application.Context.ApplicationContext.PackageManager.GetPackageInfo(Application.Context.ApplicationContext.PackageName, 0).VersionName;

         
        }

        void Rebind()
        {

            appSettings.LocalSettings = appSettings.LocalSettings.ReadXML();
            BindingContext = new FirmsViewModel() { items = appSettings.LocalSettings.Firms };

            
        }





        private void AddFirm_Clicked(object sender, EventArgs e)
        {
            FirmAdd fm = new FirmAdd();
            fm.Disappearing += Fm_Disappearing;
            Navigation.PushAsync(fm);
        }

        private void Fm_Disappearing(object sender, EventArgs e)
        {
            Rebind();
        }

        private async void Firms_Tapped(object sender, EventArgs e)
        {
 
            await Navigation.PushModalAsync(appSettings.activity);
            Task.Delay(500).Wait();
    
            var mi = sender as StackLayout;
            appSettings.ApiURL = "";
           X_Firms f = (X_Firms)((TapGestureRecognizer)mi.GestureRecognizers.First()).CommandParameter;
            appSettings.CurrentFirm = f.Server + "_" + f.DataBase;
           
            if (f.Server.StartsWith("http"))
                appSettings.ApiURL = f.Server;
            else
            {
                string connString = string.Format("Data Source={0};Initial Catalog={1};User ID={2};Password={3};Connection Timeout=5", f.Server,
       f.DataBase, f.User, f.Pass);
                TurbimSQLHelper.defaultconn = new TurbimSQLHelper(connString);
                if (TurbimSQLHelper.defaultconn.isOpen())
                {
                    appSettings.CalismaTarihi1 = ("01.01." + System.DateTime.Now.Year).convDateTime();
                    appSettings.CalismaTarihi2 = ("31.12." + System.DateTime.Now.Year).convDateTime();
                    appParams.UserSettings.DefaultConnString = connString;
                    TurbimSQLHelper.database = f.DataBase;
                    TurbimSQLHelper.server = f.Server;
                    TurbimSQLHelper.User = f.User;
                    TurbimSQLHelper.Pass = f.Pass;
                    appSettings.GoPage(nameof(LoginPage));
                }
                else
                    appSettings.UyariGoster("Lütfen bağlantı ayarlarını kontrol ediniz. İnternete bağlı olduğunuzdan emin olununuz..");
            }
            Navigation.PopModalAsync();
        }

        private async void Sil_Clicked(object sender, EventArgs e)
        {
            MenuItem mi = sender as MenuItem;
            X_Firms f = mi.CommandParameter as X_Firms;

            if (await appSettings.Onay(f.Name+ " bağlantısı silinecektir. Onaylıyor musunuz?"))
            {
                appSettings.LocalSettings.Firms.Remove(f);
                appSettings.LocalSettings.SaveXML();

                Rebind();
            }
        }
        private void Guncelle_Clicked(object sender, EventArgs e)
        {
            UpdateAPK fm = new UpdateAPK();
            Navigation.PushAsync(fm);
        }
    }
}


namespace GoldenMobileX.ViewModels
{

    public class FirmsViewModel : BaseViewModel
    {
        public Command AddItemCommand
        {
            get;
        }
        public List<X_Firms> items
        {
            get; set;
        }

 
        public X_Firms _item;
        public X_Firms item
        {
            get => _item;
            set => SetProperty(ref _item, value);
        }
 
    }
}