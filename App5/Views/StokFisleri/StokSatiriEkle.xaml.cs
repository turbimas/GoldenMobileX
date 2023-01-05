using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoldenMobileX.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using GoldenMobileX.Models;
using ZXing.Net.Mobile.Forms;
using Xamarin.Essentials;
using Android.Views;
using System.Threading;

namespace GoldenMobileX.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StokSatiriEkle : ContentPage
    {
        public int InvoiceID = 0, StockTransID = 0;
        int? Direction = 0;
        public bool edit = false;
        int DepoID = 0;
        public StokFisleriViewModel viewModel
        {
            get { return (StokFisleriViewModel)BindingContext; }
            set { BindingContext = value; }
        }
        public StokSatiriEkle()
        {

            InitializeComponent();
            this.Appearing += StokSatiriEkle_Appearing;
            ProductEntry.TextChanged += Product_TextChanged;


            string txt = "@string.cancel";


        }

 
        private void SatirEntrySeriNo_Completed(object sender, EventArgs e)
        {
            if (HizliEkle.IsToggled)
            {
                SatirKaydet_Clicked(null, null);
            }
            else
                SatirEntryAmount.Focus();
        }

        private async void StokSatiriEkle_Appearing(object sender, EventArgs e)
        {
            Fiyatlar.IsVisible = false;
            if (viewModel.Trans != null) Direction = viewModel.Trans.Type_?.Direction;
            if (viewModel.Invoice != null)
            {
                Direction = viewModel.Invoice.Type_?.Direction;
                Fiyatlar.IsVisible = true;
            }
  
            BtnKaydet.IsEnabled = false;

            if (viewModel.Line.ProductID == 0) BtnKaydet.IsEnabled = false;
            else
            {
                BtnKaydet.IsEnabled = true;

            }
            if (viewModel.Trans.Type_.Direction == 1)
                DepoID = viewModel.Trans.DestStockWareHouseID_.ID;
            else
                DepoID = viewModel.Trans.StockWareHouseID_.ID;



        }
        bool barkodOkundu = false;

        bool SeriNoKontrolEt = true;
        private void SatirEntrySeriNo_Unfocused(object sender, FocusEventArgs e)
        {
            if (SatirEntrySeriNo.Text + "" == "") return;
            List<V_AllItems> itm = DataLayer.V_AllItems.AsEnumerable().Where(x => x.Barcode == SatirEntrySeriNo.Text).ToList();
         
            if (itm.Count() == 0)
            {
                if (viewModel.Trans.Type_.Direction == 1)  //Eğer depo giriş işlemi ise seri numaralarında arama....
                {
                    appSettings.UyariGoster("Ürün bulunamadı...");
                    Vibration.Vibrate(1000);
                    return;
                }
                var Satirlar =new  List<V_DepodakiLotlar>();
                if (SeriNoKontrolEt && DataLayer.IsOnline)
                {

                    try
                    {
                        if (DataLayer.DepolakiLotlar.Count == 0)
                            DataLayer.DepolakiLotlar = DataLayer.v_DepodakiLotlar;
                        if (DataLayer.DepolakiLotlar == null) SeriNoKontrolEt = false;
                        if (DataLayer.DepolakiLotlar.Count == 0) SeriNoKontrolEt = false;
                        Satirlar = DataLayer.DepolakiLotlar.Where(s => s.Depo == DepoID && (s.SeriLot == SatirEntrySeriNo.Text || s.BalyaNo == SatirEntrySeriNo.Text || s.LotID == SatirEntrySeriNo.Text)).ToList();
                        if (Satirlar.Sum(s => s.Miktar) + viewModel.Trans.Lines.Where(s => s.ProductID == viewModel.Line.ProductID && s.SeriLot == viewModel.Line.SeriLot && s.BalyaNo == viewModel.Line.BalyaNo && s.LotID == viewModel.Line.BalyaNo).Sum(s => s.Amount * s.Direction) <= 0)
                        {
                            appSettings.UyariGoster("Bu üründen depoda yeteri kadar bulunmuyor.");
                            Vibration.Vibrate(2000);
                            return;
                        }
                    }
                    catch
                    {
                        SeriNoKontrolEt = false;
                        appSettings.UyariGoster("Seri Lot Kontrolü yapılamıyor. Ya da ürün depoda bulunamadı.");
                        Vibration.Vibrate(2000);
                        return;
                    }
                }
                else
                {
                    appSettings.UyariGoster("Ürün bulunamadı...");
                    Vibration.Vibrate(2000);
                    return;
                }
                if (Satirlar.Count() == 0)
                {
                    appSettings.UyariGoster("Ürün depoda bulunamadı...");
                    Vibration.Vibrate(2000);
                    return;
                }
                else
                {
                    var Satir = Satirlar.First();
                    ProductEntry.SelectedItem = DataLayer.V_AllItems.Where(s => s.ID == Satir.ProductID).First();
                    SatirEntrySeriLot.Text = Satir.SeriLot;
                    viewModel.Line.BalyaNo = Satir.BalyaNo;
                    viewModel.Line.LotID = Satir.LotID;
                    barkodOkundu = true;
                    Vibration.Vibrate(100);
                }
            }
            else
            {
 
                ProductEntry.SelectedItem = itm.FirstOrDefault();

                barkodOkundu = true;
                Vibration.Vibrate(100);
            }
        }

        private void Product_TextChanged(object sender, EventArgs e)
        {
            Controls.GoldenEntryProductPicker p = sender as Controls.GoldenEntryProductPicker;
            if (p.SelectedItem.ID == 0) return;
            SatirEntrySeriNo.Text = p.SelectedItem.Barcode +"";

            if (SatirEntryUnitPrice.Value.convDouble() == 0)
            {
                if (Direction == -1)
                    SatirEntryUnitPrice.Value = p.SelectedItem.UnitPrice.convDecimal();
                else
                    SatirEntryUnitPrice.Value = p.SelectedItem.AlisFiyati.convDecimal();
            }
            viewModel.Line.ProductID_ = p.SelectedItem;
            SatirEntryTaxRate.Value = p.SelectedItem.TaxRate.convDecimal();
            SatirEntryAmount.Value = null;
            SatirEntryAmount.Suffix = p.SelectedItem.UnitID_.UnitCode;
            viewModel.Line.UnitID = p.SelectedItem.UnitID_.ID;
            LabelStokAdeti.Text = "Stokta: " + p.SelectedItem.StokAdeti;
            BtnKaydet.IsEnabled = true;






            if (HizliEkle.IsToggled)
            {
                if (barkodOkundu)
                {
                    if (SatirEntryAmount.Value.convInt() == 0)
                        SatirEntryAmount.Value = (sabitmiktar > 0 ? sabitmiktar : 1).convDecimal();
                    SatirKaydet_Clicked(sender, e);
                    SatirEntryAmount.Focus();
                    Thread.Sleep(100);
                    SatirEntrySeriNo.Focus();
                }
            }
            else
            {
                SatirEntryAmount.Focus();
               
            }
            barkodOkundu = false;
            
         

        }

        private void Entry_TextChanged(object sender, EventArgs e)
        {
            SatirEntryTotal.Value = (SatirEntryAmount.Value.convDouble() * (SatirEntryUnitPrice.Value.convDouble() * (1 + (SatirEntryTaxRate.Value.convInt() / 100)) - SatirEntryDiscount.Value.convDouble())).convDecimal();
            if (MiktariSabitleSw.IsToggled)
                sabitmiktar = SatirEntryAmount.Value.convInt();
        }






        private async void SatirKaydet_Clicked(object sender, EventArgs e)
        {
            if (viewModel.Line.Amount == 0 || viewModel.Line.Amount==null)
            {
                appSettings.UyariGoster("Lütfen miktar giriniz.");
                SatirEntryAmount.Focus();
                return;
            }
            if (viewModel.Line.ProductID == 0)
            {
                appSettings.UyariGoster("Lütfen ürün seçiniz.");
                ProductEntry.Focus();
                return;
            }
            if (!await CheckList()) return;
            bool sayimResult = false;
            List<TRN_StockTransLines> ayniurun = new List<TRN_StockTransLines>();
            Parallel.ForEach(viewModel.Trans.Lines, s =>
             {
                 if (s != viewModel.Line)
                     if (s.SeriNo + "" == viewModel.Line.SeriNo + "")
                         if ((s.ProductID_?.ID).convInt() == (viewModel.Line?.ProductID_.ID).convInt())
                             if (s.SeriLot == viewModel.Line.SeriLot && s.LotID == viewModel.Line.LotID)
                                 ayniurun.Add(s);
             });
            if (viewModel.Line.CreatedBy > 0)
            {
                viewModel.Line.ModifiedBy = appSettings.User.ID;
            //Satır düzenleniyor. İşlem Yapma
            }
            else
            {
                if (ayniurun.Count() > 0)
                {
                    if (SayimModu.IsToggled)
                    {
                        string uyaritxt = "Bu üründen daha önce " + ayniurun.Sum(s => s.Amount) + " adet geçilmiş. Üzerine eklenecektir.";
                        sayimResult = await App.Current.MainPage.DisplayAlert("UYARI", uyaritxt, "Evet Ekle", "Hayır İptal Et");
                    }

                    if (!SayimModu.IsToggled || sayimResult)
                        ayniurun.First().Amount += viewModel.Line.Amount;

                }
                else
                {
                    viewModel.Line.CreatedBy = appSettings.User.ID;
                    viewModel.Trans.Lines.Add(viewModel.Line);
                }
            }
            //Yeni Satıra geç
            viewModel.Line = new TRN_StockTransLines();

            viewModel.Line.Date = DateTime.Now; //Sıralama için
            viewModel.Line.Amount = (sabitmiktar > 0 ? sabitmiktar : 1);

 

            BtnKaydet.IsEnabled = false;
            RebindSatirlar();
            appSettings.OfflineData.TRN_StockTransLines = new List<TRN_StockTransLines>(viewModel.Trans.Lines.Where(s => s.StockTransID == 0 || s.StockTransID==null));
            appSettings.OfflineData.SaveXML();
            SatirEntrySeriNo.Focus();
        }
       async Task<bool> CheckList()
        {
            if (viewModel.CheckListLines.Count() == 0) { return true; }
            else
            {
                var bulunanurun = viewModel.Trans.Lines.Where(s => s.ProductID == viewModel.Line.ProductID && s.SeriNo + "" == viewModel.Line.SeriNo + "" && s.SeriLot+"" == viewModel.Line.SeriLot+"" && s.BalyaNo+"" == viewModel.Line.BalyaNo+"");
                var checkEdilecekUrun = viewModel.CheckListLines.Where(s => s.ProductID == viewModel.Line.ProductID && s.SeriNo + "" == viewModel.Line.SeriNo + "" && s.SeriLot == viewModel.Line.SeriLot && s.BalyaNo == viewModel.Line.BalyaNo);
                if (checkEdilecekUrun.Count() > 0)
                {
                    if ((bulunanurun.FirstOrDefault()?.Amount).convDouble() + viewModel.Line.Amount.convDouble() > (checkEdilecekUrun.FirstOrDefault()?.Amount))
                    {
                        Vibration.Vibrate(2000);
                        if (!await appSettings.Onay("Bu ürün fazla gelmiş. İşleme devam edecek misiniz?")) return false;

                    }
                }
                else
                {
                    Vibration.Vibrate(2000);
                    if (!await appSettings.Onay("Bu ürün bu depo transferinde mevcut değil..Fişe eklenecektir ve sonrasına yeni bir fiş olarak kaydedilecektir.")) return false;
                  
                    
                    return true;

                }
            }
            return true;
        }
        void RebindSatirlar()
        {
            try
            {
                viewModel = new StokFisleriViewModel() { Trans = viewModel.Trans, Line = viewModel.Line, CheckListLines=viewModel.CheckListLines };
                ListViewSatirlar.ItemsSource = new List<TRN_StockTransLines>(viewModel.Trans.Lines.Where(s => s.ProductID_ != null).Where(s => s.ProductID > 0).OrderByDescending(s => s.Date).ThenByDescending(s => s.ID));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        private async void BarkodOku_Clicked(object sender, EventArgs e)
        {
            ZXingScannerPage scanPage = new ZXingScannerPage();

            scanPage.OnScanResult += (result) =>
            {
                scanPage.IsScanning = false;
                Device.BeginInvokeOnMainThread(() =>
                {
                    Navigation.PopAsync();
                    SatirEntrySeriNo.Text = result.Text;
                    SatirEntrySeriNo_Unfocused(SatirEntrySeriNo, null);
                    SatirEntrySeriNo_Completed(null, null);
                });
            };
            await Navigation.PushAsync(scanPage);
        }

 
        int sabitmiktar = 1;
        private void MiktariSabitle_Toggled(object sender, ToggledEventArgs e)
        {
            if ((sender as Switch).IsToggled)
                sabitmiktar = SatirEntryAmount.Value.convInt();
            else
                sabitmiktar = 1;
        }

        private void Duzenle_Clicked(object sender, EventArgs e)
        {
            var mi = sender as SwipeItem;
            TRN_StockTransLines l = (TRN_StockTransLines)mi.CommandParameter;

            viewModel.Line = l;
     
            RebindSatirlar();
        }

        private async void Sil_Clicked(object sender, EventArgs e)
        {
            var mi = sender as SwipeItem;
         
            if (await appSettings.Onay())
            {
                viewModel.Trans.Lines.Remove((TRN_StockTransLines)mi.CommandParameter);
                RebindSatirlar();
            }
        }

        private void BtnStokEkle_Clicked(object sender, EventArgs e)
        {
            StokKarti fm = new StokKarti();
            
            Navigation.PushAsync(fm);
        }

        private void SatirEntryAmount_Completed(object sender, EventArgs e)
        {
            SatirKaydet_Clicked(null, null);
        }


    }
}