using GoldenMobileX.Models;
using GoldenMobileX.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoldenMobileX.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StokFisleri : ContentPage
    {
        GoldenContext c = new GoldenContext();
        StokFisleriViewModel viewModel
        {
            get { return (StokFisleriViewModel)BindingContext; }
        }
        public StokFisleri()
        {
            InitializeComponent();
            this.BindingContext = new StokFisleriViewModel();

            Appearing += StokFisleri_Appearing;
        }

        private void StokFisleri_Appearing(object sender, EventArgs e)
        {
            Rebind();
        }

        DataTable items = new DataTable();
        void Rebind()
        {
            if(DataLayer.IsOfflineAlert) { return; }
            try
            {
                IsBusy = true;
                int warehouse = appSettings.User.WareHouseID.convInt();
                var t = c.TRN_StockTrans.Where(s => s.CreatedBy == appSettings.User.ID || s.DestStockWareHouseID == warehouse || s.StockWareHouseID == warehouse).Select(s=>s).OrderByDescending(s => s.ID);


                viewModel.TransList = DataLayer.WaitingSent.tRN_StockTrans.OrderByDescending(s => s.TransDate).ToList();
                viewModel.TransList.ForEach(s => s.Duzenlenebilir = true);
                this.BindingContext = new StokFisleriViewModel() { TransList = new List<TRN_StockTrans>(viewModel.TransList), CheckListLines = new List<TRN_StockTransLines>() };
                IsBusy = false;
            }
            catch (Exception ex)
            {
                appSettings.UyariGoster(ex.Message);
                IsBusy = false;
            }
            finally
            {
                IsBusy = false;
            }

        }

        private async void YeniFis_Clicked(object sender, EventArgs e)
        {
            StokFisi fm = new StokFisi();
            fm.newAdd = true;
            appSettings.OfflineData = appSettings.OfflineData.ReadXML();
            if (appSettings.OfflineData.TRN_StockTransLines.Count > 0)
            {
                if (!(await appSettings.Onay("Daha önceden kaydedilmemiş satırlar var. Eklemek istermisiniz.")))
                {
                    appSettings.OfflineData.TRN_StockTransLines = new List<TRN_StockTransLines>();
                    appSettings.OfflineData.SaveXML();
                }
            }

            viewModel.Trans = new TRN_StockTrans()
            {
                FicheNo = appSettings.GetCounter("StokFisi", "TRN_StockTrans", "FicheNo", "TRN" + DateTime.Now.Year),
                TransDate = DateTime.Now,
                Lines = appSettings.OfflineData.TRN_StockTransLines,
                Status = appParams.Genel.YeniMalzemeFislerindeDurum,
                Branch = appSettings.UserDefaultBranch
            };
            fm.viewModel = viewModel;
            fm.Disappearing += Fm_Disappearing;

            Navigation.PushAsync(fm);


        }

        private void Fm_Disappearing(object sender, EventArgs e)
        {
            Rebind();
        }



        private void Refresh_Clicked(object sender, EventArgs e)
        {
            DataLayer.TRN_StockTransGelenTransfer();
            Rebind(); DataLayer.WaitingSent.SaveJSON();
        }



        private void Duzenle_Clicked(object sender, EventArgs e)
        {
            var mi = sender as SwipeItem;
            TRN_StockTrans t = (TRN_StockTrans)mi.CommandParameter;
            StokFisi fm = new StokFisi();
            fm.Disappearing += Fm_Disappearing;


            if (t.Status_?.Code != 4)
            {
                appSettings.UyariGoster("Bu fiş onaylanmış. Değişiklik Yapamazsınız.");
                return;
            }

            viewModel.Trans = t;

            if (t.Status_?.Code == 4 && t.Type_.Direction == 0 && DataLayer.Depolar.Where(s => s.ID == t.DestStockWareHouseID_.ID).Count() > 0 && t.ID > 0) // Kullanıcının depoya giriş yetkisi var ise
            {
                var newt = new TRN_StockTrans() { CreatedBy = appSettings.User.ID, CreatedDate = DateTime.Now, Type_ = t.Type_, DestStockWareHouseID_ = t.DestStockWareHouseID_, StockWareHouseID_ = t.StockWareHouseID_, TransDate = DateTime.Now, FicheNo = appSettings.GetCounter("Depo", "TRN_StockTransLines", "FicheNo") };
                newt.Lines = new List<TRN_StockTransLines>();
                viewModel.CheckListLines = t.Lines;
                viewModel.Trans = newt;
                viewModel.Trans.Lines = new List<TRN_StockTransLines>();

            }
            fm.viewModel = viewModel;
            Navigation.PushAsync(fm);
        }


        private async void Sil_Clicked(object sender, EventArgs e)
        {
            if (!await appSettings.Onay("Fiş Silinecektir..")) return;
            var mi = sender as SwipeItem;
            TRN_StockTrans t = (TRN_StockTrans)mi.CommandParameter;
 
            Rebind();

        }

        private void Onayla_Clicked(object sender, EventArgs e)
        {
            if (DataLayer.IsOfflineAlert) return;
            var mi = sender as SwipeItem;
            TRN_StockTrans t = (TRN_StockTrans)mi.CommandParameter;
            if (t.Type_.Direction != 0)
            {
                appSettings.UyariGoster("Bu kaydı onaylayamazsınız..");
                return;
            }
            if ((t.DestStockWareHouseID_?.ID).convInt() == appSettings.User.WareHouseID)
            {
                t.Status = 1;
                c.TRN_StockTrans.Update(t);
                c.SaveContextWithException();
                Rebind();
            }
            else
                appSettings.UyariGoster("Bu kaydı onaylayamazsınız..");
        }

        private void Goruntule_Clicked(object sender, EventArgs e)
        {
            if (DataLayer.IsOfflineAlert) { return; }
                var mi = sender as SwipeItem;
            TRN_StockTrans t = (TRN_StockTrans)mi.CommandParameter;
            StokFisi fm = new StokFisi();
            viewModel.Trans = t;
            if (t.ID > 0)
                if (t.Lines == null || t.Lines?.Count() == 0)
                {
                 
                        try
                        {
                            using (GoldenContext c = new GoldenContext())
                            {
                                t.Lines = c.TRN_StockTransLines.Where(s => s.StockTransID == t.ID).Select(s => s).ToList();
                            }
                        }
                        catch (Exception ex)
                        {
                            ex.UyariGoster();
                        }
                }
            fm.viewModel = viewModel;
            fm.ReadOnly = true;
            Navigation.PushAsync(fm);
        }

 
 
    }
}


