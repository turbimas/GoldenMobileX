using GoldenMobileX.Models;
using GoldenMobileX.ViewModels;
using System;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


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

        private async void Button_Clicked(object sender, EventArgs e)
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



        private async void login()
        {
            await Navigation.PushModalAsync(appSettings.activity);

            if (DataLayer.IsOfflineAlert) return;
            await Device.InvokeOnMainThreadAsync(() => appSettings.activity.viewModel = new BaseViewModel() { activityText = "Giriş yapılıyor.." });

            using (GoldenContext c = new GoldenContext())
            {
                var user = c.X_Users.Where(s => s.LogonName == EntryUserName.Text && s.Password == EntryPassword.Text).FirstOrDefault();

                string yetkiler = "";
                if (user != null)
                {
                    appSettings.User = user;
                    appSettings.UserAuthCode = (yetkiler + user.AuthCode).Replace(" ", "");
                    await Device.InvokeOnMainThreadAsync(() => appSettings.activity.viewModel = new BaseViewModel() { activityText = "Statik data yükleniyor.." });


                    DataLayer.LoadStaticData();
                    await Device.InvokeOnMainThreadAsync(() => appSettings.activity.viewModel = new BaseViewModel() { activityText = "Menü yetkileri yükleniyor.." });


                    appSettings.MenuYetkileri();
                    appSettings.GoPage(nameof(AboutPage));
                }
                else
                {
                    appSettings.UyariGoster("Kullanıcı adı ya da şifreniz hatalı...");
                }
            }
            Task.Delay(1000);
            Navigation.PopModalAsync();
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
        private void Cikis_Clicked(object sender, EventArgs e)
        {


        }
        private void Guncelle_Clicked(object sender, EventArgs e)
        {
            VeritabaniKarsilastir();
            Guncelle();
        }

        void VeritabaniKarsilastir()
        {
            string nameSpace = "GoldenMobileX.Models";
            string uymayanlar = "";
            Assembly asm = Assembly.GetExecutingAssembly();
            int i = 0;
            foreach (Type c in asm.GetTypes().Where(type => type.Namespace == nameSpace).Select(type => type).ToList())
            {
                if (!c.Name.StartsWith("CRD_") && !c.Name.StartsWith("TRN_") && !c.Name.StartsWith("X_") && !c.Name.StartsWith("L_") && !c.Name.StartsWith("V_") && !c.Name.StartsWith("W_")) continue;

                System.Reflection.PropertyInfo[] properties = c.GetProperties();
                DataTable dt = db.SQLSelectToDataTable(string.Format("SELECT DATA_TYPE, COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='{0}' ", c.Name));
                if (dt == null) continue;
                if (dt.Rows.Count == 0) continue;
                foreach (var property in properties)
                {
                    CultureInfo culture = new CultureInfo("en");
                    if (property.PropertyType.FullName.Contains(nameSpace) || property.PropertyType.FullName.Contains("System.Drawing.Color")) continue;
                    DataRow[] drs = dt.Select(string.Format("COLUMN_NAME='{0}'", property.Name));
                    if (drs.Count() == 0)
                    {
                        i++;
                        uymayanlar += c.Name + "." + property.Name + @" alanı veritabanında bulunamadı...
";
                        continue;
                    }
                    string tableDataType = drs[0]["DATA_TYPE"] + "";

                    SqlDbType type = db.ConverSqlDbType(property.PropertyType);
                    if ((type + "").ToLower(culture) == "nvarchar" && tableDataType.ToLower(culture) == "varchar") continue;
                    if ((type + "").ToLower(culture) == "datetime" && tableDataType.ToLower(culture) == "date") continue;
                    if ((type + "").ToLower(culture) != tableDataType.ToLower(culture))
                    {
                        i++;
                        uymayanlar += c.Name + "." + property.Name + "   v=> " + tableDataType + " m=>" + type + @"
";
                        updateDataTypeSQL += c.Name + "," + property.Name + "," + (((type + "").ToLower(culture) == "decimal") ? "decimal(18,3)" : type + "") + ";";

                    }
                }
            }
            if (uymayanlar != "")
                appSettings.UyariGoster(uymayanlar, i + " adet uyumsuzluk bulundu... ");
        }
        string updateDataTypeSQL = "";
        void Guncelle()
        {
            //Dispatcher.BeginInvokeOnMainThread(() => loginActivity.IsRunning = true);
            string sql = @"SELECT 'IF EXISTS(SELECT * FROM sys.objects WHERE name='''+o.name+''')  ALTER TABLE ' + t.name +' DROP CONSTRAINT ' +  o.name + '' q FROM sys.objects o, sys.tables AS t WHERE o.parent_object_id = t.[object_id] 
AND o.name LIKE 'DF_%' AND 
o.type='D' ";
            DataTable dt = db.SQLSelectToDataTable(sql);
            int i = 0;
            if (dt != null)
                foreach (DataRow dr in dt.Rows)
                {
                    db.SQLExecuteNonQuery(dr["q"] + "");

                }



            i = 0;

            foreach (string str in sql.Split(';'))
            {
                var t = str.Split(',');
                if (t.Length == 3)
                    db.SQLExecuteNonQuery(string.Format("IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='{0}' AND COLUMN_NAME='{1}' AND DATA_TYPE='{2}') ALTER TABLE {0} ALTER COLUMN {1} {2} NULL;", t[0], t[1], t[2]));
                i++;
            }
            // Dispatcher.BeginInvokeOnMainThread(() => loginActivity.IsRunning = false);
        }

        private void Karsilastir_Clicked(object sender, EventArgs e)
        {
            VeritabaniKarsilastir();
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
        private void OnLoginClicked(object obj)
        {


        }
    }
}