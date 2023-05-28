using GoldenMobileX.Models;
using System;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoldenMobileX.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class GonderAl : ContentPage
    {
        public GonderAl()
        {
            InitializeComponent();
            Button bt = new Button()
            {
                Text = (appSettings.OfflineData.SQLListToRun.Count > 0 ? "Bekleyen " + appSettings.OfflineData.SQLListToRun.Count + " sorgu işlemi var. Sorguları tekrar çalıştırmak için tıklayınız." : "Bekleyen işlem yok")
            };
            bt.Clicked += Bt_Clicked;
            StackContent.Children.Add(bt);

            Button bt2 = new Button()
            {
                Text = (appSettings.OfflineData.JSON_TRN_StockTransListToRun.Count > 0 ? "Bekleyen " + appSettings.OfflineData.JSON_TRN_StockTransListToRun.Count + " stok kayıt işlemi var. Kayıtları tekrar aktarmak için tıklayınız." : "Bekleyen işlem yok")
            };
            bt2.Clicked += BtStokKayit_Clicked;
            StackContent.Children.Add(bt2);

        }

        private async void BtStokKayit_Clicked(object sender, EventArgs e)
        {
            int i = 0;
            foreach (string str in appSettings.OfflineData.JSON_TRN_StockTransListToRun)
            {

                StokFisi fm = new StokFisi();

                fm.viewModel = new ViewModels.StokFisleriViewModel() { Trans = TurbimJSON.deSerializeObject<TRN_StockTrans>(str) };
                fm.Disappearing += (s2, e2) => Fm_Disappearing(s2, e2, i);
                await Navigation.PushAsync(fm);

                i++;
            }
            appSettings.OfflineData.SaveXML();
        }

        private async void Fm_Disappearing(object sender, EventArgs e, int i)
        {
            if (await appSettings.Onay("Bu işlemi bekleyen işlemlerden çıkaralım mı?"))
            {
                appSettings.OfflineData.JSON_TRN_StockTransListToRun.Remove(appSettings.OfflineData.JSON_TRN_StockTransListToRun[i]);
            }
        }

        private void Bt_Clicked(object sender, EventArgs e)
        {
            int i = 0;
            foreach (string str in appSettings.OfflineData.SQLListToRun)
            {
                if (db.SQLExecuteNonQuery(str) == "")
                    appSettings.OfflineData.SQLListToRun.Remove(appSettings.OfflineData.SQLListToRun[i]);
                i++;
            }
            appSettings.OfflineData.SaveXML();
        }



        private void Guncelle_Clicked(object sender, EventArgs e)
        {
            DataLayer.LoadDataFromSQL(true);
            appSettings.UyariGoster("Tamamlandı...");
        }

        private void Temizle_Clicked(object sender, EventArgs e)
        {

            DataLayer.LocalDataBase.ExecuteAsync("DELETE FROM V_AllItems");
            DataLayer.v_DepodakiLotlar.Clear();
            DataLayer.LocalDataBase.ExecuteAsync("DELETE FROM V_DepodakiLotlar");

        }

        private void OtomatikSenkronize_CheckedChanged(object sender, EventArgs e)
        {
            appSettings.LocalSettings.SaveXML();
        }
    }
}