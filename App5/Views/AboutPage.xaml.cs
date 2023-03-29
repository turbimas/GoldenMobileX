using System;
using Xamarin.Forms;

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
            if (DataLayer.IsOnline)
            {
                ListViewBankalar.ItemsSource = DataLayer.DataTableToObject<Models.Bilanco>(db.SQLSelectToDataTable(@"SELECT SUM(x.Bakiye) TUTAR,'BANKALAR' HESAP, 1 as SIRA FROM
(
SELECT SUM(H.Bakiye) Bakiye FROM CRD_BankaHesaplari H WHERE H.CurrencyID=0   --TL Toplam
UNION
SELECT SUM(H.DovizBakiye) * (SELECT Top 1 E.Rate1 FROM TRN_DailyExchange E WHERE E.CurrencyID=1 ORDER BY E.Date DESC) FROM  CRD_BankaHesaplari H WHERE H.CurrencyID=1     --USD Toplam
UNION
SELECT SUM(H.DovizBakiye) * (SELECT Top 1 E.Rate1 FROM TRN_DailyExchange E WHERE E.CurrencyID=20 ORDER BY E.Date DESC) FROM  CRD_BankaHesaplari H WHERE H.CurrencyID=20 
)x  
UNION
SELECT ISNULL(SUM(H.Tutar),0), 'PORTFÖYDEKİ ÇEK SENET', 2 FROM TRN_CekSenetler H WHERE H.VadeTarihi>getdate() AND H.Durumu=0

UNION
SELECT SUM(x.Bakiye),'KASALAR' HESAP, 3 FROM
(
SELECT SUM(H.DovizTutari) Bakiye FROM V_KasaHareketleri H, CRD_Kasalar K WHERE K.ID=H.KasaID AND K.CurrencyID=0    --TL Toplam
UNION 
SELECT SUM(H.DovizTutari) * (SELECT Top 1 E.Rate1 FROM TRN_DailyExchange E WHERE E.CurrencyID=1 ORDER BY E.Date DESC) Bakiye FROM V_KasaHareketleri H, CRD_Kasalar K WHERE K.ID=H.KasaID AND K.CurrencyID=1    --TL Toplam
UNION 
SELECT SUM(H.DovizTutari) * (SELECT Top 1 E.Rate1 FROM TRN_DailyExchange E WHERE E.CurrencyID=20 ORDER BY E.Date DESC) Bakiye FROM V_KasaHareketleri H, CRD_Kasalar K WHERE K.ID=H.KasaID AND K.CurrencyID=2    --TL Toplam
)x  
UNION


SELECT SUM(x.Bakiye),'ALICILAR' HESAP,4 FROM
(
SELECT SUM(V.Bakiye) Bakiye FROM V_Cari V, CRD_Cari C WHERE C.ID=V.ID AND C.CardType=1 AND C.CurrencyID=0 AND C.Active=1   --TL Toplam
UNION
SELECT SUM(V.USDBakiye) * (SELECT Top 1 E.Rate1 FROM TRN_DailyExchange E WHERE E.CurrencyID=1 ORDER BY E.Date DESC)  FROM V_Cari V, CRD_Cari C WHERE C.ID=V.ID AND C.CardType=1 AND C.CurrencyID=1 AND C.Active=1  --USD Toplam
UNION
SELECT SUM(V.EUROBakiye)* (SELECT Top 1 E.Rate1 FROM TRN_DailyExchange E WHERE E.CurrencyID=20 ORDER BY E.Date DESC)   FROM V_Cari V, CRD_Cari C WHERE C.ID=V.ID AND C.CardType=1 AND C.CurrencyID=20 AND C.Active=1  
)x 
UNION

SELECT SUM(x.Bakiye), 'SATICILAR',5 FROM
(
SELECT SUM(V.Bakiye) Bakiye FROM V_Cari V, CRD_Cari C WHERE C.ID=V.ID AND C.CardType=2 AND C.CurrencyID=0 AND C.Active=1   --TL Toplam
UNION
SELECT SUM(V.USDBakiye) * (SELECT Top 1 E.Rate1 FROM TRN_DailyExchange E WHERE E.CurrencyID=1 ORDER BY E.Date DESC)  FROM V_Cari V, CRD_Cari C WHERE C.ID=V.ID AND C.CardType=2 AND C.CurrencyID=1 AND C.Active=1  --USD Toplam
UNION
SELECT SUM(V.EUROBakiye)* (SELECT Top 1 E.Rate1 FROM TRN_DailyExchange E WHERE E.CurrencyID=20 ORDER BY E.Date DESC)   FROM V_Cari V, CRD_Cari C WHERE C.ID=V.ID AND C.CardType=2 AND C.CurrencyID=20 AND C.Active=1  
)x 
UNION
SELECT SUM(PacalMaliyet* StokAdeti), 'STOKLAR',6 FROM CRD_Items WHERE Type=1 AND StokAdeti>0

ORDER BY 3
"));
            }
        }

        private async void Button_Clicked(object sender, EventArgs e)
        {

            appSettings.GoPage(nameof(Firms));
            //await Browser.OpenAsync("http://www.goldenerp.com", BrowserLaunchMode.SystemPreferred);
        }


    }
}