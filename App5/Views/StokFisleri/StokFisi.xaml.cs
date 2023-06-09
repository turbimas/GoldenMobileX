﻿using Android.Content;
using GoldenMobileX.Models;
using GoldenMobileX.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Transactions;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;


namespace GoldenMobileX.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StokFisi : ContentPage
    {
        public bool ReadOnly = false;
        public bool newAdd = false;
        GoldenContext c = new GoldenContext();
        public StokFisleriViewModel viewModel
        {
            get { return (StokFisleriViewModel)BindingContext; }
            set { BindingContext = value; }
        }
        public StokFisi()
        {
            InitializeComponent();
            this.Appearing += StokFisi_Appearing;

        }

        private void FisPickerCikis_SelectedIndexChanged(object sender, EventArgs e)
        {
            FisKaydetEnabled();
        }

        private async void StokFisi_Appearing(object sender, EventArgs e)
        {
            if (ReadOnly)
            {
                BtnSatirEkle.IsVisible = false;
                ToolbarItems.Remove(BtnKaydet);
                StackStokFisi.IsEnabled = false;
            }
            RebindSatir();
            FisKaydetEnabled();

        }

        private async void FisKaydet_Clicked(object sender, EventArgs e)
        {
            if (!await CheckList())
                Kaydet();

        }
        void Kaydet()
        {
            if (DataLayer.IsOfflineAlert) return;

            if (viewModel.Trans?.Lines != null)
            {
                viewModel.Trans.Lines.RemoveAll(s => s.Amount == 0);
            }
            if (viewModel.Trans.Status_ == null)
                viewModel.Trans.Status = appParams.Genel.YeniMalzemeFislerindeDurum;
            else if (viewModel.Trans.Status_?.Code == 0)
                viewModel.Trans.Status = appParams.Genel.YeniMalzemeFislerindeDurum;
            using (var transaction = new TransactionScope())
            {
                try
                {
                    foreach (var s in viewModel.Trans.Lines)
                    {
                        s.UnitPrice = s.ProductID_.UnitPrice;
                        s.Direction = viewModel.Trans.Type_.Direction;
                        s.Total = s.UnitPrice.convDecimal() * s.Amount;
                    }
                    viewModel.Trans.Total = viewModel.Trans.Lines.Sum(x => x.Total).convDouble(2);
                    if (viewModel.Trans.ID > 0)
                    {
                        viewModel.Trans.ModifiedBy = appSettings.User.ID;
                        viewModel.Trans.ModifiedDate = DateTime.Now;
                        c.TRN_StockTrans.Update(viewModel.Trans);
                    }
                    else
                    {
                        viewModel.Trans.CreatedBy = appSettings.User.ID;
                        viewModel.Trans.CreatedDate = DateTime.Now;
                        c.TRN_StockTrans.Add(viewModel.Trans);
                    }
                    c.SaveChanges();
                    foreach (var l in viewModel.Trans.Lines)
                    {
                        l.StockTransID = viewModel.Trans.ID;
                        if (l.ID > 0)
                            c.TRN_StockTransLines.Update(l);
                        else
                            c.TRN_StockTransLines.Add(l);

                    }
                    c.SaveChanges();

                    // İşlem başarılıysa onayla
                    transaction.Complete();
                }
                catch (Exception ex)
                {
                    // Hata durumunda geri al
                    Console.WriteLine("Hata: " + ex.Message);
                }
            }
            Navigation.PopAsync();

        }
        async Task<bool> CheckList()  //Depo transferinde eksik ya da fazla gelen ürünlere fiş oluşturur.
        {
            if (viewModel.CheckListLines.Count() == 0) { return false; }

            DataLayer.WaitingSent.tRN_StockTrans.Where(s => s.ID == viewModel.CheckListLines.FirstOrDefault().StockTransID).FirstOrDefault().Status = 6;
            await appSettings.UyariGoster("Gelen fiş onaylanmıştır.");
            List<TRN_StockTransLines> eksikGelenUrunler = new List<TRN_StockTransLines>();
            //Gelmeyen ürünleri bul
            foreach (var Line in viewModel.CheckListLines)
            {
                var listedekiUrun = viewModel.Trans.Lines.Where(s => s.ProductID == Line.ProductID && s.SeriNo + "" == Line.SeriNo + "" && s.SeriLot == Line.SeriLot && s.BalyaNo == Line.BalyaNo);
                if (listedekiUrun.Count() == 0)  // Ürün gelmemiş. Geri iade transferi oluştur.
                {
                    eksikGelenUrunler.Add(Line);
                }
            }
            //Eksik gelen ürünleri hesapla
            foreach (var Line in viewModel.Trans.Lines)
            {
                var checkEdilecekUrun = viewModel.CheckListLines.Where(s => s.ProductID == Line.ProductID && s.SeriNo + "" == Line.SeriNo + "" && s.SeriLot == Line.SeriLot && s.BalyaNo == Line.BalyaNo);
                if (checkEdilecekUrun.Count() > 0)  // Ürün listede var.
                {
                    if (Line.Amount == checkEdilecekUrun.FirstOrDefault().Amount)
                    {
                        Line.Amount = 0;
                        continue;
                    }
                    else if (Line.Amount > checkEdilecekUrun.FirstOrDefault().Amount)
                    {
                        Line.Amount = Line.Amount - checkEdilecekUrun.FirstOrDefault().Amount;
                    }
                    else if (Line.Amount < checkEdilecekUrun.FirstOrDefault().Amount)
                    {
                        Line.Amount = checkEdilecekUrun.FirstOrDefault().Amount - Line.Amount;
                        eksikGelenUrunler.Add(Line);
                        Line.Status = -1;
                        continue;
                    }
                }
                else  //Ürün listede yok. Yeni fişe gelen ürün olarak ekle.
                {

                }
            }
            viewModel.Trans.Lines.RemoveAll(s => s.Status == -1);


            var GelenFis = viewModel.TransList.Where(s => s.ID == viewModel.CheckListLines.FirstOrDefault().StockTransID).FirstOrDefault();

            //Fazla gelen ürünleri bul
            if (viewModel.Trans.Lines.Count() > 0)
            {
                viewModel.Trans.Status = 6;
                viewModel.Trans.Notes = GelenFis.StockWareHouseID_.Name + " DEPOSUNDAN FAZLA GELEN ÜRÜNLER.";
                newAdd = true;
                Kaydet();
                await appSettings.UyariGoster("Fazladan gelen ürünler vardır. Bunlar farklı bir fişte gelen fiş olarak onaylı bir şekilde kaydedilmiştir..");
            }

            if (eksikGelenUrunler.Count() > 0)
            {
                var newt = new TRN_StockTrans()
                {
                    Status = 4,
                    CreatedBy = appSettings.User.ID,
                    CreatedDate = DateTime.Now,
                    Type_ = viewModel.Trans.Type_,
                    DestStockWareHouseID_ = viewModel.Trans.StockWareHouseID_,
                    StockWareHouseID_ = viewModel.Trans.DestStockWareHouseID_,
                    TransDate = DateTime.Now,
                    FicheNo = appSettings.GetCounter("Depo", "TRN_StockTransLines", "FicheNo"),
                    Lines = eksikGelenUrunler,
                    Notes = GelenFis.StockWareHouseID_.Name + " DEPOSUNDAN EKSİK GELEN ÜRÜNLER."
                };

                viewModel.Trans = newt;
                await appSettings.UyariGoster("Eksik gelen ürünler vardır. Bunlar farklı bir fişte gönderilen ürünler olarak taşıma sürecinde durumu ile kaydedilmiştir.. Karşı taraf eksik gelen ürünleri onayladığında kendi deposuna giriş işlemi yapılacaktır. Bu işlem sizin deponuzu etkilemez.");
                newAdd = true;
                Kaydet();

            }

            return true;
        }
        bool FisKaydetEnabled()
        {
            bool kaydet = false;
            if ((FisPickerType.SelectedItem as X_Types) != null)
            {
                int? Direction = (FisPickerType.SelectedItem as X_Types)?.Direction;
                CRD_StockWareHouse StockWareHouse = FisPickerCikis.SelectedItem as CRD_StockWareHouse;
                CRD_StockWareHouse DestStockWareHouse = FisPickerGiris.SelectedItem as CRD_StockWareHouse;
                if (Direction == 1)
                {
                    if (DestStockWareHouse != null) kaydet = true;
                    FisPickerCikis.SelectedItem = null;
                }
                if (Direction == -1)
                {
                    if (StockWareHouse != null) kaydet = true;
                    FisPickerGiris.SelectedItem = null;
                }
                if (Direction == 0)
                {
                    if (StockWareHouse != null && DestStockWareHouse != null) kaydet = true;
                }
            }
            BtnKaydet.IsEnabled = kaydet;
            BtnSatirEkle.IsEnabled = kaydet;
            return kaydet;
        }
        private void SatirEkle_Clicked(object sender, EventArgs e)
        {
            if (!FisKaydetEnabled())
            {
                appSettings.UyariGoster("Lütfen önce ilgili depoyu seçiniz..");
                return;
            }
            if ((FisPickerType.SelectedItem as X_Types) == null)
            {
                appSettings.UyariGoster("Lütfen önce fiş türünü seçiniz..");
                return;
            }
            StokSatiriEkle fm = new StokSatiriEkle();
            viewModel.Line = new TRN_StockTransLines();
            viewModel.Line.Date = DateTime.Now;
            fm.viewModel = viewModel; //new StokFisleriViewModel() { Trans = viewModel.Trans, Line=viewModel.Line }; 

            Navigation.PushAsync(fm);
        }
        private void Duzenle_Clicked(object sender, EventArgs e)
        {
            var mi = sender as SwipeItem;
            TRN_StockTransLines l = (TRN_StockTransLines)mi.CommandParameter;


            StokSatiriEkle fm = new StokSatiriEkle();
            viewModel.Line = l;
            l.Date = DateTime.Now;
            fm.viewModel = viewModel;
            Navigation.PushAsync(fm);

        }
        void RebindSatir()
        {
            if (viewModel.Trans?.Lines != null)
            {
                viewModel.Trans.Lines.RemoveAll(s => s.ProductID == 0);
                ListViewSatirlar.ItemsSource = new List<TRN_StockTransLines>(viewModel.Trans.Lines).OrderByDescending(s => s.Date).ThenByDescending(s => s.ID);
            }
        }






        private void PickerType_SelectedIndexChanged(object sender, EventArgs e)
        {

            if (FisPickerType.SelectedItem == null) return;
            if (viewModel.Trans.Lines != null)
                foreach (TRN_StockTransLines l in viewModel.Trans.Lines)
                {
                    l.Type = (FisPickerType.SelectedItem as X_Types).Code.convInt();
                    l.Direction = (FisPickerType.SelectedItem as X_Types).Direction.convInt();
                }
            int? Direction = (FisPickerType.SelectedItem as X_Types).Direction;
            FisPickerGiris.IsVisible = true;
            FisPickerCikis.IsVisible = true;
            if (Direction == -1)
            {
                FisPickerGiris.IsVisible = false;
            }
            if (Direction == 1)
            {
                FisPickerCikis.IsVisible = false;
            }
            FisKaydetEnabled();

            CariList.IsVisible = ((new int[] { 0, 1, 9, 10, 11 }).Where(x => x == viewModel.Trans.Type).Count() > 0);

        }

        private async void Sil_Clicked(object sender, EventArgs e)
        {
            var mi = sender as SwipeItem;

            if (await appSettings.Onay())
            {
                viewModel.Trans.Lines.Remove((TRN_StockTransLines)mi.CommandParameter);
                RebindSatir();
            }
        }

    }
}