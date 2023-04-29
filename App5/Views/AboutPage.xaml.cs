using System;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Linq;
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
            return;
            if (DataLayer.IsOnline)
            {
                try
                {
                    List<Models.Bilanco> b = DataLayer.DataTableToObject<Models.Bilanco>(db.SQLSelectToDataTable(@"
SELECT SUM(x.Bakiye) TUTAR,'BANKALAR' HESAP, 102 as SIRA FROM
(
SELECT SUM(H.Bakiye) Bakiye FROM CRD_BankaHesaplari H WHERE H.CurrencyID=0   --TL Toplam
UNION
SELECT SUM(H.DovizBakiye) * (SELECT Top 1 E.Rate1 FROM TRN_DailyExchange E WHERE E.CurrencyID=1 ORDER BY E.Date DESC) FROM  CRD_BankaHesaplari H WHERE H.CurrencyID=1     --USD Toplam
UNION
SELECT SUM(H.DovizBakiye) * (SELECT Top 1 E.Rate1 FROM TRN_DailyExchange E WHERE E.CurrencyID=20 ORDER BY E.Date DESC) FROM  CRD_BankaHesaplari H WHERE H.CurrencyID=20 
)x  
UNION
SELECT ISNULL(SUM(H.Tutar),0), 'PORTFÖYDEKİ ÇEK SENET', 101 FROM TRN_CekSenetler H WHERE H.VadeTarihi>getdate() AND H.Durumu=0

UNION
SELECT SUM(x.Bakiye),'KASALAR' HESAP, 100 FROM
(
SELECT SUM(H.DovizTutari) Bakiye FROM V_KasaHareketleri H, CRD_Kasalar K WHERE K.ID=H.KasaID AND K.CurrencyID=0    --TL Toplam
UNION 
SELECT SUM(H.DovizTutari) * (SELECT Top 1 E.Rate1 FROM TRN_DailyExchange E WHERE E.CurrencyID=1 ORDER BY E.Date DESC) Bakiye FROM V_KasaHareketleri H, CRD_Kasalar K WHERE K.ID=H.KasaID AND K.CurrencyID=1    --TL Toplam
UNION 
SELECT SUM(H.DovizTutari) * (SELECT Top 1 E.Rate1 FROM TRN_DailyExchange E WHERE E.CurrencyID=20 ORDER BY E.Date DESC) Bakiye FROM V_KasaHareketleri H, CRD_Kasalar K WHERE K.ID=H.KasaID AND K.CurrencyID=2    --TL Toplam
)x  
UNION


SELECT SUM(x.Bakiye),'ALICILAR' HESAP,120 FROM
(
SELECT SUM(V.Bakiye) Bakiye FROM V_Cari V, CRD_Cari C WHERE C.ID=V.ID AND C.CardType=1 AND C.CurrencyID=0 AND C.Active=1 HAVING SUM(V.Bakiye)>0   --TL Toplam
UNION
SELECT SUM(V.USDBakiye) * (SELECT Top 1 E.Rate1 FROM TRN_DailyExchange E WHERE E.CurrencyID=1 ORDER BY E.Date DESC)  FROM V_Cari V, CRD_Cari C WHERE C.ID=V.ID AND C.CardType=1 AND C.CurrencyID=1 AND C.Active=1 HAVING SUM(V.USDBakiye)>0   --USD Toplam
UNION
SELECT SUM(V.EUROBakiye)* (SELECT Top 1 E.Rate1 FROM TRN_DailyExchange E WHERE E.CurrencyID=20 ORDER BY E.Date DESC)   FROM V_Cari V, CRD_Cari C WHERE C.ID=V.ID AND C.CardType=1 AND C.CurrencyID=20 AND C.Active=1  HAVING SUM(V.EUROBakiye)>0 
)x 
UNION

SELECT SUM(x.Bakiye), 'SATICILAR',320 FROM
(
SELECT SUM(V.Bakiye) Bakiye FROM V_Cari V, CRD_Cari C WHERE C.ID=V.ID AND C.CardType=2 AND C.CurrencyID=0 AND C.Active=1   HAVING SUM(V.Bakiye)<0   --TL Toplam
UNION
SELECT SUM(V.USDBakiye) * (SELECT Top 1 E.Rate1 FROM TRN_DailyExchange E WHERE E.CurrencyID=1 ORDER BY E.Date DESC)  FROM V_Cari V, CRD_Cari C WHERE C.ID=V.ID AND C.CardType=2 AND C.CurrencyID=1 AND C.Active=1 HAVING SUM(V.USDBakiye)<0   --USD Toplam
UNION
SELECT SUM(V.EUROBakiye)* (SELECT Top 1 E.Rate1 FROM TRN_DailyExchange E WHERE E.CurrencyID=20 ORDER BY E.Date DESC)   FROM V_Cari V, CRD_Cari C WHERE C.ID=V.ID AND C.CardType=2 AND C.CurrencyID=20 AND C.Active=1    HAVING SUM(V.EUROBakiye)<0 
)x 
UNION
SELECT        SUM((I.AlisFiyati * I.StokAdeti * ISNULL(R.Rate1, 1)) * (1 + I.TaxRate / 100)) AS Expr1, 'STOKLAR (KDV DAHİL)' AS Expr2, 153 AS Expr3
FROM            CRD_Items AS I LEFT OUTER JOIN
                         V_CurrencyRates AS R ON I.DovizKodu = R.CurrencyID
WHERE        (I.Type = 1) AND (I.StokAdeti >= 1)

ORDER BY 3


"));
                    ListViewBankalar.ItemsSource = b;
                  
                    BtnToplam.Text = "VARLIK: " + b.Sum(s => s.TUTAR).convDouble().ToString("n2") + " TL";
                }
                catch(Exception ex)
                {
                    appSettings.UyariGoster(ex.Message);
                }
            }



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
    }
}