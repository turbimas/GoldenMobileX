using GoldenMobileX.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

using GoldenMobileX.Views;
using System.Data;
using GoldenMobileX.Models;
using Xamarin.Forms.PlatformConfiguration;
using Xamarin.Forms.Internals;
using System.Globalization;

namespace GoldenMobileX.Views
{
 

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            if (appSettings.User == null) appSettings.User = new X_Users() { ID = 0 };
            if(appSettings.User.ID>0)
                appSettings.GoPage(nameof(AboutPage));
            InitializeComponent();
            this.BindingContext = new LoginViewModel();
            SwitchBeniHatirla.IsToggled = appParams.UserSettings.BeniHatirla;
            if(appParams.UserSettings.BeniHatirla)
            {
                EntryUserName.Text = appParams.UserSettings.UserName;
                EntryPassword.Text = appParams.UserSettings.Password;
            }

            CultureInfo customCulture = (CultureInfo)System.Threading.Thread.CurrentThread.CurrentCulture.Clone();
            customCulture.NumberFormat.NumberDecimalSeparator = ",";
            customCulture.NumberFormat.NumberGroupSeparator = ".";



            System.Threading.Thread.CurrentThread.CurrentCulture = customCulture;
            //ActivitiyStatus.IsRunning = false;
        }

        private void Button_Clicked(object sender, EventArgs e)
        {


 
            if (SwitchBeniHatirla.IsToggled)
            {
                appParams.UserSettings.UserName = EntryUserName.Text;
                appParams.UserSettings.Password = EntryPassword.Text;

                appParams.UserSettings.BeniHatirla = true;
            }
            else
                appParams.UserSettings.BeniHatirla = false;


            login();



        }
 
        void login()
        {
            if (DataLayer.IsOfflineAlert) return;
            
            Task.Run(() => loginActivity.IsRunning = true).Wait();
            using (GoldenContext c = new GoldenContext())
            {
                var user = c.X_Users.Where(s => s.LogonName == EntryUserName.Text && s.Password == EntryPassword.Text).FirstOrDefault();
 
                string yetkiler = "";
                if (user != null)
                {
                    appSettings.User = user;
                    appSettings.UserAuthCode = (yetkiler + user.AuthCode).Replace(" ", "");
                    DataLayer.LoadStaticData();
                    appSettings.MenuYetkileri();
                    appSettings.GoPage(nameof(AboutPage));
                }
                else
                {
                    appSettings.UyariGoster("Kullanıcı adı ya da şifreniz hatalı...");
                }
            }
            Task.Run(() => loginActivity.IsRunning = false).Wait();
        }
        private  void  ToolbarItem_Clicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if(await App.Current.MainPage.DisplayAlert("Kullanıcı ayarları silinecektir. Onaylıyor musunuz..", "Uyarı", "Evet", "Hayır"))
                {
                    appSettings.DeleteUserData();
                    appSettings.GoPage(nameof(Firms));
                }


            });
 
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
    public class LoginViewModel : BaseViewModel
    {
        private string userName, userPassword;
        public Command LoginCommand { get; }


        public LoginViewModel()
        {
            LoginCommand = new Command(OnLoginClicked);
        }
        public string UserName
        {
            get => userName;
            set => SetProperty(ref userName, value);
        }

        public string UserPassword
        {
            get => userPassword;
            set => SetProperty(ref userPassword, value);
        }
        private  void OnLoginClicked(object obj)
        {


        }
    }
}