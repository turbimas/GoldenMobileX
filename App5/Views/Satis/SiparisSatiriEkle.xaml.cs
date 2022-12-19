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
namespace GoldenMobileX.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SiparisSatiriEkle : ContentPage
    {
        public int InvoiceID = 0, StockTransID = 0;
        int? Direction = 0;
        public bool edit = false;
        public OrdersViewModel viewModel
        {
            get { return (OrdersViewModel)BindingContext; }
            set { BindingContext = value; }
        }
        public SiparisSatiriEkle()
        {
            InitializeComponent();
            this.Appearing += SiparisSatiriEkle_Appearing;
            ProductEntry.TextChanged += Product_TextChanged;
        }

        private void SiparisSatiriEkle_Appearing(object sender, EventArgs e)
        {
            Direction = viewModel.Order.OrderType_?.Direction;

            SatirEntrySeriNo.Focus();
            BtnKaydet.IsEnabled = false;

            if (viewModel.Line.ProductID == 0) BtnKaydet.IsEnabled = false;
            else
            {
                BtnKaydet.IsEnabled = true;

            }
            if (viewModel.Order.OrderType_?.Code == 1)
                FiyatBilgisiStackLayout.IsVisible = false;

        }

        private void SatirEntrySeriNo_Unfocused(object sender, FocusEventArgs e)
        {
            if (SatirEntrySeriNo.Text + "" == "") return;
            List<V_AllItems> itm = DataLayer.V_AllItems.AsEnumerable().Where(x => x.Barcode == SatirEntrySeriNo.Text).ToList();
            if (itm.Count() == 0)
                appSettings.UyariGoster("Ürün bulunamadı...");
            else
            {
                ProductEntry.SelectedItem = itm.FirstOrDefault();
                SatirEntryAmount.Value = 1;
            }
        }

        private void Entry_TextChanged(object sender, EventArgs e)
        {
            SatirEntryTotal.Value = (SatirEntryAmount.Value.convDouble() * (SatirEntryUnitPrice.Value.convDouble() * (1 + (SatirEntryTaxRate.Value.convInt() / 100)) - SatirEntryDiscount.Value.convDouble())).convDecimal();
        }


        private void Product_TextChanged(object sender, EventArgs e)
        {
            Controls.GoldenEntryProductPicker p = sender as Controls.GoldenEntryProductPicker;
            if (p.SelectedItem.ID == 0) return;
            SatirEntrySeriNo.Text = p.SelectedItem.Barcode + "";


            if (SatirEntryUnitPrice.Value.convDouble() == 0)
            {
                if (Direction == -1)
                    SatirEntryUnitPrice.Value = (p.SelectedItem as V_AllItems).UnitPrice.convDecimal();
                else
                    SatirEntryUnitPrice.Value = (p.SelectedItem as V_AllItems).AlisFiyati.convDecimal();


            }
            SatirEntryTaxRate.Value = p.SelectedItem.TaxRate.convDecimal();
            SatirEntryAmount.Suffix = p.SelectedItem.UnitID_.UnitCode;
            viewModel.Line.UnitID = p.SelectedItem.UnitID_.ID;
            LabelStokAdeti.Text = "Stokta: " + p.SelectedItem.StokAdeti;
            BtnKaydet.IsEnabled = true;

            SatirEntryAmount.Value = 1;
        }



        private async void SatirKaydet_Clicked(object sender, EventArgs e)
        {

            bool sayimResult = false;
            var ayniurun = viewModel.Order.Lines.Where(s => s.ProductID_ != null).Where(s => s != viewModel.Line && s.SeriNo + "" == viewModel.Line.SeriNo + "" && s.ProductID == ((sender as Picker).SelectedItem as V_AllItems).ID);

            if (viewModel.Line.ID == -1)
            {

            }
            else
            {
                if (ayniurun.Count() > 0)
                {
                    string uyaritxt = "Bu üründen daha önce " + ayniurun.Sum(s => s.Amount) + " adet geçilmiş. Üzerine eklenecektir.";
                    sayimResult = await App.Current.MainPage.DisplayAlert("UYARI", uyaritxt, "Evet Ekle", "Hayır İptal Et");
                    if (sayimResult)
                        ayniurun.First().Amount += viewModel.Line.Amount;

                }
                else
                {
                    viewModel.Line.ID = -1;
                    viewModel.Order.Lines.Add(viewModel.Line);
                }
            }

            viewModel.Line = new TRN_OrderLines();


            BindingContext = new OrdersViewModel() { Order = viewModel.Order, Line = viewModel.Line };
            SatirEntrySeriNo.Focus();
            BtnKaydet.IsEnabled = false;
            RebindSatirlar();

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
                    SatirEntrySeriNo_Unfocused(null, null);
                });
            };
            await Navigation.PushAsync(scanPage);
        }

        private async void SatirSil_Clicked(object sender, EventArgs e)
        {
            MenuItem mi = sender as SwipeItem;
            if (await appSettings.Onay())
            {
                viewModel.Order.Lines.Remove((TRN_OrderLines)mi.CommandParameter);

                DataLayer.TRN_OrderLines(((TRN_OrderLines)mi.CommandParameter).ID);
            }
            RebindSatirlar();
        }
        void RebindSatirlar()
        {
            try
            {
                viewModel = new  OrdersViewModel() { Order = viewModel.Order, Line = viewModel.Line };
        

                ListViewSatirlar.ItemsSource = new List<TRN_OrderLines>(viewModel.Order.Lines.Where(s => s.ProductID_ != null).Where(s => s.ProductID > 0).OrderBy(s => s.ID));
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private void Satir_Clicked(object sender, EventArgs e)
        {
            var mi = sender as SwipeItem;
            TRN_OrderLines l = (TRN_OrderLines)mi.CommandParameter;

            viewModel.Line = l;

            RebindSatirlar();

        }

        private async void Ara_Clicked(object sender, EventArgs e)
        {
            Stoklar fm = new Stoklar();

            fm.ItemSelected += (s2, e2) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Navigation.PopAsync();
                    ProductEntry.SelectedItem = fm.item;
                });
            };
            await Navigation.PushAsync(fm);
        }
    }
}