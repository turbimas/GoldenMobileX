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
            if (appSettings.User.ID > 0)
                appSettings.GoPage(nameof(AboutPage));
            InitializeComponent();
            this.BindingContext = new LoginViewModel();
            SwitchBeniHatirla.IsToggled = appParams.UserSettings.BeniHatirla;
            if (appParams.UserSettings.BeniHatirla)
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
            /*
            Task.Run(() =>
            {
                loginActivity.IsVisible = true;
                btnLogin.Text = "Lütfen bekleyiniz...";
            }).Wait();
            */
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

        }
        private void ToolbarItem_Clicked(object sender, EventArgs e)
        {
            Device.BeginInvokeOnMainThread(async () =>
            {
                if (await App.Current.MainPage.DisplayAlert("Kullanıcı ayarları silinecektir. Onaylıyor musunuz..", "Uyarı", "Evet", "Hayır"))
                {
                    appSettings.DeleteUserData();
                    appSettings.GoPage(nameof(Firms));
                }


            });

        }

        private void Guncelle_Clicked(object sender, EventArgs e)
        {
          
            Guncelle();
        }
        void Guncelle()
        {
            p1.Run();
   
            string sql = @"SELECT 'IF EXISTS(SELECT * FROM sys.objects WHERE name='''+o.name+''')  ALTER TABLE ' + t.name +' DROP CONSTRAINT ' +  o.name + '' q FROM sys.objects o, sys.tables AS t WHERE o.parent_object_id = t.[object_id] 
AND o.name LIKE 'DF_%' AND 
o.type='D' ";
            DataTable dt = db.SQLSelectToDataTable(sql);
            int i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                db.SQLExecuteNonQuery(dr["q"] + "");

                p1.PopupText = i + " / " + dt.Rows.Count;
                i++;
            }

            sql = @"SELECT G.TABLE_NAME, G.COLUMN_NAME, G.DATA_TYPE, 

'IF EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='''+G.TABLE_NAME+''' 
AND COLUMN_NAME='''+G.COLUMN_NAME+''' AND DATA_TYPE=''decimal'')
ALTER TABLE '+ G.TABLE_NAME +' ALTER COLUMN ' + G.COLUMN_NAME +' FLOAT NULL;' q
FROM  INFORMATION_SCHEMA.COLUMNS G  WHERE
(G.DATA_TYPE='float' OR G.DATA_TYPE='decimal') AND G.TABLE_NAME NOT LIKE 'V_%'  AND G.TABLE_NAME NOT LIKE 'Z_%' 
";

            dt = db.SQLSelectToDataTable(sql);
            i = 0;
            foreach (DataRow dr in dt.Rows)
            {
                db.SQLExecuteNonQuery(dr["q"] + "");
                p1.PopupText = i + " / " + dt.Rows.Count;
                i++;
            }

            p1.Stop();
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