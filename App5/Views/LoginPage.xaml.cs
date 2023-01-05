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

 
using System.Reflection;
 

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
                if (dt.Rows.Count==0) continue;
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
                    }
                }
            }
            if (uymayanlar != "")
                appSettings.UyariGoster( uymayanlar, i + " adet uyumsuzluk bulundu... ");
        }
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

            sql = @"

BilgiIslem_CihazListesi,EcsBedeli,float, 
BilgiIslem_CihazListesi,SatinAlmaBedeli,float, 
BilgiIslem_Lisanslar,Tutar,float, 
BilgiIslem_TonerKayit,KDVDahilFiyat_TL,float, 
CRD_Accounts,ButceTutari,float, 
CRD_Accounts,Credit,float, 
CRD_Accounts,Debit,float, 
CRD_BankaHesaplari,Bakiye,float, 
CRD_BankaHesaplari,BalonOdemeTaksitTutari,float, 
CRD_BankaHesaplari,BSMV,float, 
CRD_BankaHesaplari,DovizBakiye,float, 
CRD_BankaHesaplari,FaizOrani,float, 
CRD_BankaHesaplari,KKDF,float, 
CRD_BankaHesaplari,KrediKartiLimit,float, 
CRD_BankaHesaplari,KrediTutari,float, 
CRD_BankaHesaplari,Masraf,float, 
CRD_BankaHesaplari,ToplamOdenecekTutar,float, 
CRD_Cari,Bakiye,float, 
CRD_Cari,CekSenetBakiye,float, 
CRD_Cari,DovizliBakiye,float, 
CRD_Cari,EUROBakiye,float, 
CRD_Cari,IndirimOrani,float, 
CRD_Cari,PuanBakiye,float, 
CRD_Cari,Risk,float, 
CRD_Cari,USDBakiye,float, 
CRD_CariAdres,lat,float, 
CRD_CariAdres,lng,float, 
CRD_ItemBarcodes,Fiyat,float, 
CRD_ItemBarcodes,Fiyat2,float, 
CRD_ItemBarcodes,Fiyat3,float, 
CRD_ItemBarcodes,Miktar,float, 
CRD_ItemBarcodes,StokAdeti,float, 
CRD_ItemPrices,SatisFiyati,float, 
CRD_ItemProperties,DefaultValue,float, 
CRD_ItemProperties,MaxValue,float, 
CRD_ItemProperties,MinValue,float, 
CRD_Items,Agirlik,float, 
CRD_Items,AgirlikGr,float, 
CRD_Items,AlisFiyati,float, 
CRD_Items,AmortismanOrani,float, 
CRD_Items,BalyadakiPaket,float, 
CRD_Items,Bonus,float, 
CRD_Items,BoyCm,float, 
CRD_Items,DefaultAmount,float, 
CRD_Items,Derinlik,float, 
CRD_Items,EnCm,float, 
CRD_Items,HedefKarMarji,float, 
CRD_Items,HurdaDegeri,float, 
CRD_Items,IskontoluMinSatisTutari,float, 
CRD_Items,KritikSeviye,float, 
CRD_Items,MaksimumSeviye,float, 
CRD_Items,MinPrice,float, 
CRD_Items,PacalMaliyet,float, 
CRD_Items,PakettekiMiktar,float, 
CRD_Items,RezervStokAdeti,float, 
CRD_Items,StokAdeti,float, 
CRD_Items,StokCikis,float, 
CRD_Items,StokGiris,float, 
CRD_Items,TaxRate,float, 
CRD_Items,TaxRateToptan,float, 
CRD_Items,UnitPrice,float, 
CRD_Items,UnitPrice2,float, 
CRD_Items,UnitPrice3,float, 
CRD_Items,UretimVarsayilanMiktar,float, 
CRD_Items,WebAddFactor,float, 
CRD_ItemWareHouses,KritikSeviye,float, 
CRD_ItemWareHouses,MaksimumSeviye,float, 
CRD_ItemWareHouses,SatisFiyati,float, 
CRD_ProductBOM,BakımGideri,float, 
CRD_ProductBOM,ElektrikGideri,float, 
CRD_ProductBOM,HesaplanacakMiktar,float, 
CRD_ProductBOM,IscilikGideri,float, 
CRD_ProductionOperations,BakımGideri,float, 
CRD_ProductionOperations,ElektrikGideri,float, 
CRD_ProductionOperations,HesaplanacakMiktar,float, 
CRD_ProductionOperations,IscilikGideri,float, 
CRD_ProductionOperations,MakineFiyati,float, 
CRD_StockWareHouse,Kapasite,float, 
CRD_Vehicles,AlisFiyati,float, 
CRD_Vehicles,DevirMasrafi,float, 
CRD_Vehicles,SatisFiyati,float, 
Kalite_AnalizOlcumTanimlari,MaxValue,float, 
Kalite_AnalizOlcumTanimlari,MinValue,float, 
Kalite_AnalizOlcumVerileri,SayisalDegeri,float, 
Kalite_Risk,Katsayi,float, 
Kalite_RiskRevizyon,Katsayi,float, 
L_Units,Cofactor,float, 
L_VehicleVersion,MotorHacmi,float, 
Lojistik_Operasyonlar,KDV,decimal,(18,9)
Lojistik_Operasyonlar,Toplam,decimal,(18,9)
Lojistik_Operasyonlar,Tutar,decimal,(18,9)
MSG_Hatirlatmalar,Amount,float, 
MSG_Hatirlatmalar,CurrencyRate,float, 
MSG_Hatirlatmalar,UnitPrice,float, 
OSB_Abonelikler,Katsayi,float, 
OSB_Abonelikler,Miktar,float, 
Personel_Bordro,Agi,float, 
Personel_Bordro,BES,float, 
Personel_Bordro,BrutUcret,float, 
Personel_Bordro,DamgaVergisi,float, 
Personel_Bordro,FazlaMesaiSaati,float, 
Personel_Bordro,FazlaMesaiUcreti,float, 
Personel_Bordro,GelirVergisi,float, 
Personel_Bordro,IssizlikIsciPrimi,float, 
Personel_Bordro,IssizlikIsverenPrimi,float, 
Personel_Bordro,KumulatifMatrah,float, 
Personel_Bordro,Matrah,float, 
Personel_Bordro,NetUcret,float, 
Personel_Bordro,NormalMesai,float, 
Personel_Bordro,PrimGunu,float, 
Personel_Bordro,SakatlikIndirimi,float, 
Personel_Bordro,SSKIsciPrimi,float, 
Personel_Bordro,SSKIsverenPrimi,float, 
Personel_GelirVergisiDilimleri,MaksimumTutar,float, 
Personel_GelirVergisiDilimleri,MinimumTutar,float, 
Personel_GelirVergisiDilimleri,VergiOrani,float, 
Personel_Izin,ToplamGun,float, 
Personel_Parametreler,AgiEsOrani,float, 
Personel_Parametreler,AsgariUcret,float, 
Personel_Parametreler,AsgariUcretCarpani,float, 
Personel_Parametreler,CocukKatsayisi0,float, 
Personel_Parametreler,CocukKatsayisi1,float, 
Personel_Parametreler,CocukKatsayisi2,float, 
Personel_Parametreler,CocukKatsayisi3,float, 
Personel_Parametreler,CocukKatsayisi4,float, 
Personel_Parametreler,CocukKatsayisi5,float, 
Personel_Parametreler,DamgaVergisi,float, 
Personel_Parametreler,EmekliIssizlikIsciPrimi,float, 
Personel_Parametreler,EmekliIssizlikIsverenPrimi,float, 
Personel_Parametreler,EmekliNetOran,float, 
Personel_Parametreler,EmekliSSKIsciPrimi,float, 
Personel_Parametreler,EmekliSSKIsverenPrimi,float, 
Personel_Parametreler,Ikinci6AySGKTavani,float, 
Personel_Parametreler,Ilk6AySGKTavani,float, 
Personel_Parametreler,IssizlikIsciPrimi,float, 
Personel_Parametreler,IssizlikIsverenPrimi,float, 
Personel_Parametreler,MaxAsgariOran,float, 
Personel_Parametreler,NetOran,float, 
Personel_Parametreler,SakatlikInd1,float, 
Personel_Parametreler,SakatlikInd2,float, 
Personel_Parametreler,SakatlikInd3,float, 
Personel_Parametreler,SakatlikKatsayisi,float, 
Personel_Parametreler,SGKTavanFarki,float, 
Personel_Parametreler,SGKTavanFarki2,float, 
Personel_Parametreler,SGKTavanFarki3,float, 
Personel_Parametreler,SGKTavanFarki4,float, 
Personel_Parametreler,SGKTavani,float, 
Personel_Parametreler,SGKTavani2,float, 
Personel_Parametreler,SGKTavani3,float, 
Personel_Parametreler,SGKTavani4,float, 
Personel_Parametreler,SGKTavanKatSayis2,float, 
Personel_Parametreler,SGKTavanKatSayisi,float, 
Personel_Parametreler,SGKTavanKatSayisi3,float, 
Personel_Parametreler,SGKTavanKatSayisi4,float, 
Personel_Parametreler,SSKIsciPrimi,float, 
Personel_Parametreler,SSKIsverenPrimi,float, 
Personel_SicilKartlari,BesOrani,float, 
Personel_SicilKartlari,EkGelir,float, 
Personel_SicilKartlari,Maasi,float, 
RENT_Hasar,HasarTutari,float, 
RENT_Hasar,SigortadanTahsilEdilecekTutar,float, 
RENT_Hasar,Tutar,float, 
RENT_Kiralamalar,KiraBedeli,float, 
RENT_KiraTaksitleri,KiraTurari,float, 
RENT_KrediOdemeleri,BSMV,float, 
RENT_KrediOdemeleri,KalanAnaPara,float, 
RENT_KrediOdemeleri,KKDF,float, 
RENT_KrediOdemeleri,OdemeTutari,float, 
RENT_KrediOdemeleri,OdenenAnaPara,float, 
RENT_KrediOdemeleri,OdenenFaiz,float, 
RENT_KrediOdemeleri,TLTutari,float, 
RENT_ListeFiyatlari,AltiAylikMTV,float, 
RENT_ListeFiyatlari,AracYolMasrafi,float, 
RENT_ListeFiyatlari,AylikKrediFaizi,float, 
RENT_ListeFiyatlari,BalonOdemeTaksitTutari,float, 
RENT_ListeFiyatlari,BSMV,float, 
RENT_ListeFiyatlari,IkinciElDegeri,float, 
RENT_ListeFiyatlari,KullanilanKredi,float, 
RENT_ListeFiyatlari,Muayene,float, 
RENT_ListeFiyatlari,PlanlananKarOrani,float, 
RENT_ListeFiyatlari,SatinalmaMaliyeti,float, 
RENT_ListeFiyatlari,TaksitTutari,float, 
RENT_ListeFiyatlari,YillikBakim,float, 
RENT_ListeFiyatlari,YillikKasko,float, 
RENT_ListeFiyatlari,YillikLastik,float, 
RENT_ListeFiyatlari,YillikTrafik,float, 
RENT_MTVTutarlari,Tutar,float, 
RENT_Sozlesmeler,KM_Asim_Carpani_TL,float, 
RENT_Sozlesmeler,MusteriOnayFiyati,float, 
RENT_Sozlesmeler,TeklifTutari,float, 
RENT_Teklif,KM_Asim_Carpani_TL,float, 
RENT_Teklif,MusteriOnayFiyati,float, 
RENT_Teklif,TeklifTutari,float, 
SatinAlma_GelenFaturalar,Tutar,float, 
SatinAlma_OdemePlani,OdemeTutari,float, 
SatinAlma_Sarf,Miktar,float, 
SatinAlma_Satirlar,BirimFiyat,float, 
SatinAlma_Satirlar,DovizKuru,float, 
SatinAlma_Satirlar,Miktar,float, 
SatinAlma_Satirlar,TahminiBirimFiyat,float, 
SatinAlma_Satirlar,TeslimAlinanMiktar,float, 
SatinAlma_Siparis,VerilenSiparisAvansi,float, 
TRN_AccountLines,Miktar,float, 
TRN_AracTalepFormu,FaizOrani,float, 
TRN_AracTalepFormu,KrediTutari,float, 
TRN_BankaHareketleri,BSMV,float, 
TRN_BankaHareketleri,DovizKuru,float, 
TRN_BankaHareketleri,KalanAnaPara,float, 
TRN_BankaHareketleri,KKDF,float, 
TRN_BankaHareketleri,OdenenAnaPara,float, 
TRN_BankaHareketleri,OdenenFaiz,float, 
TRN_BankaHareketleri,Tutar,float, 
TRN_ButceDonemleri,Gerceklesen,float, 
TRN_ButceDonemleri,Hedef,float, 
TRN_CariHesapFisleri,Alacak,float, 
TRN_CariHesapFisleri,Borc,float, 
TRN_CariHesapFisleri,CurrencyRate,float, 
TRN_CariHesapHareketleri,Alacak,float, 
TRN_CariHesapHareketleri,AnaPara,float, 
TRN_CariHesapHareketleri,BirimFiyat,float, 
TRN_CariHesapHareketleri,Borc,float, 
TRN_CariHesapHareketleri,CurrencyRate,float, 
TRN_CariHesapHareketleri,Faiz,float, 
TRN_CariHesapHareketleri,Miktar,float, 
TRN_CariMutabakat,KarsiTutar,float, 
TRN_CariMutabakat,Tutar,float, 
TRN_CekSenetBordrolari,DovizKuru,float, 
TRN_CekSenetHareketleri,IslemDovizKuru,float, 
TRN_CekSenetHareketleri,KarsiIslemDovizKuru,float, 
TRN_CekSenetler,DovizKuru,float, 
TRN_CekSenetler,KarsiIslemDovizKuru,float, 
TRN_CekSenetler,Tutar,float, 
TRN_CustomerPoints,CurrencyRate,float, 
TRN_CustomerPoints,Tutar,float, 
TRN_DailyExchange,Rate1,float, 
TRN_DailyExchange,Rate2,float, 
TRN_DailyExchange,Rate3,float, 
TRN_DailyExchange,Rate4,float, 
TRN_EInvoice,Total,float, 
TRN_EksikListesiSatirlar,DepoMiktar,float, 
TRN_EksikListesiSatirlar,SatisOrt,float, 
TRN_EksikListesiSatirlar,SecondAlisFiyati,float, 
TRN_EksikListesiSatirlar,SonAlisFiyati,float, 
TRN_EksikListesiSatirlar,Talep,float, 
TRN_EtiketBasimEmirleri,EskiFiyat,float, 
TRN_EtiketBasimEmirleri,Fiyat,float, 
TRN_EtiketBasimEmirleri,Fiyat2,float, 
TRN_EtiketBasimEmirleri,Fiyat3,float, 
TRN_EtiketBasimEmirleri,MainUnitPrice,float, 
TRN_EtiketBasimEmirleri,SonAlisFiyati,float, 
TRN_FaturaKapatma,DovizKuru,float, 
TRN_FaturaKapatma,KapananTutar,float, 
TRN_Ihracat,BalyaAdet,float, 
TRN_Ihracat,BrutAgirlik,float, 
TRN_Ihracat,DovizKuru,float, 
TRN_Ihracat,NavlunUcreti,float, 
TRN_Ihracat,NetAgirlik,float, 
TRN_Ihracat,SigortaUcreti,float, 
TRN_Invoice,BagkurPrim,float, 
TRN_Invoice,BorsaTasdik,float, 
TRN_Invoice,CurrencyRate,float, 
TRN_Invoice,Discount1,float, 
TRN_Invoice,Discount2,float, 
TRN_Invoice,Discount3,float, 
TRN_Invoice,NakitOdenen,float, 
TRN_Invoice,Nakliye,float, 
TRN_Invoice,NetTotal,float, 
TRN_Invoice,PuanTutar,float, 
TRN_Invoice,StopajOrani,float, 
TRN_Invoice,Total,float, 
TRN_Invoice,TotalDiscount,float, 
TRN_Invoice,TotalTax,float, 
TRN_KasaFisleri,CurrencyRate,float, 
TRN_KasaFisleri,Tutar,float, 
TRN_KasaHareketleri,DovizKuru,float, 
TRN_KasaHareketleri,Tutar,float, 
TRN_OdemePlani,OdemeTutari,float, 
TRN_OrderLines,Agirlik,float, 
TRN_OrderLines,AlisFiyati,float, 
TRN_OrderLines,Amount,float, 
TRN_OrderLines,Bonus,float, 
TRN_OrderLines,Boy,float, 
TRN_OrderLines,CurrencyRate,float, 
TRN_OrderLines,Derinlik,float, 
TRN_OrderLines,En,float, 
TRN_OrderLines,Indirim,float, 
TRN_OrderLines,IndirimsizBirimFiyat,float, 
TRN_OrderLines,Masraf,float, 
TRN_OrderLines,PromosyonTutari,float, 
TRN_OrderLines,Total,float, 
TRN_OrderLines,UnitPrice,float, 
TRN_OrderLines,Yogunluk,float, 
TRN_Orders,CurrencyRate,float, 
TRN_Orders,IndirimOrani,float, 
TRN_Orders,KarMarji,float, 
TRN_Orders,MasrafOrani,float, 
TRN_Orders,SevkMiktari,float, 
TRN_Orders,SiparisIcinUretilenMiktar,float, 
TRN_Orders,SiparisMiktari,float, 
TRN_Orders,ToplamIndirim,float, 
TRN_Orders,ToplamMasraf,float, 
TRN_Orders,Total,float, 
TRN_Planlama,Amount,float, 
TRN_Planlama,CurrencyRate,decimal,(18,9)
TRN_Planlama,TaxRate,float, 
TRN_Planlama,Total,float, 
TRN_Planlama,TotalTax,float, 
TRN_Planlama,UnitPrice,float, 
TRN_Policeler,Total,float, 
TRN_PosSync,Amount,float, 
TRN_PosSync,CashTotal,float, 
TRN_PosSync,CashValue,float, 
TRN_PosSync,CreditCartValue,float, 
TRN_PosSync,CustomerValue,float, 
TRN_PosSync,Discount,float, 
TRN_PosSync,DiscountRate,float, 
TRN_PosSync,FNetTotal,float, 
TRN_PosSync,FoodValue,float, 
TRN_PosSync,FTotal,float, 
TRN_PosSync,FTotalDiscount,float, 
TRN_PosSync,FTotalTax,float, 
TRN_PosSync,LineTotal,float, 
TRN_PosSync,PromosyonTutari,float, 
TRN_PosSync,ShoppingCardValue,float, 
TRN_PosSync,TaxRate,float, 
TRN_PosSync,Total,float, 
TRN_PosSync,TotalTax,float, 
TRN_PosSync,UnitPrice,float, 
TRN_PosSyncReal,Amount,float, 
TRN_PosSyncReal,CashTotal,float, 
TRN_PosSyncReal,CashValue,float, 
TRN_PosSyncReal,CreditValue,float, 
TRN_PosSyncReal,CustomerValue,float, 
TRN_PosSyncReal,Discount,float, 
TRN_PosSyncReal,DiscountRate,float, 
TRN_PosSyncReal,KK1,float, 
TRN_PosSyncReal,KK10,float, 
TRN_PosSyncReal,KK2,float, 
TRN_PosSyncReal,KK3,float, 
TRN_PosSyncReal,KK4,float, 
TRN_PosSyncReal,KK5,float, 
TRN_PosSyncReal,KK6,float, 
TRN_PosSyncReal,KK7,float, 
TRN_PosSyncReal,KK8,float, 
TRN_PosSyncReal,KK9,float, 
TRN_PosSyncReal,PromosyonTutari,float, 
TRN_PosSyncReal,PuanOran,float, 
TRN_PosSyncReal,TaxRate,float, 
TRN_PosSyncReal,Total,float, 
TRN_PosSyncReal,TotalTax,float, 
TRN_PosSyncReal,UnitPrice,float, 
TRN_Production,GerceklesenMiktar,float, 
TRN_Production,PlanlananMiktar,float, 
TRN_ProductionLines,Amount,float, 
TRN_ProductionLines,CompletedAmount,float, 
TRN_ProductionLines,FireMiktari,float, 
TRN_Promosyon,FiyatYadaIndirim,float, 
TRN_Promosyon,Miktar,float, 
TRN_Promosyon,PromosyonUrunMiktar,float, 
TRN_Promosyon,Tutar,float, 
TRN_PurchaseConditionLines,Discount1,float, 
TRN_PurchaseConditionLines,Discount2,float, 
TRN_PurchaseConditionLines,Discount3,float, 
TRN_PurchaseConditionLines,NetPrice,float, 
TRN_PurchaseConditionLines,Price,float, 
TRN_StockTrans,Agirlik,float, 
TRN_StockTrans,BalyaMiktar,float, 
TRN_StockTrans,BosAgirlik,float, 
TRN_StockTrans,CurrencyRate,float, 
TRN_StockTrans,GelenFistekiAgirlik,float, 
TRN_StockTrans,KonteynerAzamiYuk,float, 
TRN_StockTrans,KonteynerDara,float, 
TRN_StockTrans,Total,float, 
TRN_StockTrans,TotalTax,float, 
TRN_StockTransLines,AggCost,float, 
TRN_StockTransLines,Agirlik,float, 
TRN_StockTransLines,Amount,float, 
TRN_StockTransLines,Boy,float, 
TRN_StockTransLines,CurrencyRate,float, 
TRN_StockTransLines,Dara,float, 
TRN_StockTransLines,Derinlik,float, 
TRN_StockTransLines,Discount,float, 
TRN_StockTransLines,Discount2,float, 
TRN_StockTransLines,Discount3,float, 
TRN_StockTransLines,DiscountRate,float, 
TRN_StockTransLines,DiscountRate2,float, 
TRN_StockTransLines,DiscountRate3,float, 
TRN_StockTransLines,En,float, 
TRN_StockTransLines,ExtraInvoiceDiscount,float, 
TRN_StockTransLines,LineTotal,float, 
TRN_StockTransLines,MalKabulAmount,float, 
TRN_StockTransLines,OrderAmount,float, 
TRN_StockTransLines,PacalMaliyet,float, 
TRN_StockTransLines,PakettekiAdet,float, 
TRN_StockTransLines,PerakendeFiyati,float, 
TRN_StockTransLines,PromosyonTutari,float, 
TRN_StockTransLines,PuanOran,float, 
TRN_StockTransLines,RealAmount,float, 
TRN_StockTransLines,TaxRate,float, 
TRN_StockTransLines,Total,float, 
TRN_StockTransLines,TotalTax,float, 
TRN_StockTransLines,UnitPrice,float, 
TRN_StockTransLines,Yogunluk,float, 
TRN_StockTransLinesExpense,Total,float, 
TRN_Terazi,Fiyat,float, 
TRN_Terazi,Miktar,float, 
TRN_Terazi,Toplam,float, 
TRN_WorkOrders,Amount,float, 
TRN_Yenilemeler,Fiyat,float, 
W_Baskets,Quantity,decimal,(18,9)
W_Evaluations,RatingValue,float, 
X_Branchs,Lat,float, 
X_Branchs,Lng,float, 
X_Duyurular,CaseNo,float, 
X_FaizOranlari,FaizOrani,float, 
X_MalzemeIhtiyaclari,AnaUrunSiparisMiktari,float, 
X_MalzemeIhtiyaclari,SiparisIhtiyaci,float, 
X_MalzemeIhtiyaclari,StokMiktari,float, 
X_OdemeTahsilatPlanSatirlari,OdemeTutari,float, 
X_OdemeTahsilatPlanSatirlari,OdemeYuzdesi,float, 
X_PacalMaliyet_1,PacalMaliyet,float, 

";

            i = 0;

            foreach (string str in sql.Split(';'))
            {
                var t = str.Split(',');
                if(t.Length==4)
                db.SQLExecuteNonQuery(string.Format("IF NOT EXISTS(SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME='{0}' AND COLUMN_NAME='{1}' AND DATA_TYPE='{2}') ALTER TABLE {0} ALTER COLUMN {1} {2}{3} NULL;", t[0], t[1], t[2], t[3]));
                i++;
            }
           // Dispatcher.BeginInvokeOnMainThread(() => loginActivity.IsRunning = false);
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