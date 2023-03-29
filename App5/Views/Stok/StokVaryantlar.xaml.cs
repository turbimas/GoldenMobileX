using GoldenMobileX.Models;
using GoldenMobileX.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Net.Mobile.Forms;

namespace GoldenMobileX.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class StokVaryantlar : ContentPage
    {
        public StoklarViewModel viewModel
        {
            get { return (StoklarViewModel)BindingContext; }
            set { BindingContext = value; }
        }
        public string title = "Varyantlar";
        public StokVaryantlar()
        {
            InitializeComponent();
            Appearing += StokVaryantlar_Appearing;
        }

        private void StokVaryantlar_Appearing(object sender, EventArgs e)
        {
            UrunAdiTxt.Text = title;
        }

        private void BtnKaydet_Clicked(object sender, EventArgs e)
        {
            if (DataLayer.V_AllItems.Where(s => s.Barcode == viewModel.varyant.Barkod).Count() > 0)
            {
                appSettings.UyariGoster("Bu barkod kullanımda");
                return;
            }
            viewModel.varyant.UrunID = viewModel.item.ID;
            if (DataLayer.IsOfflineAlert) return;
            using (GoldenContext c = new GoldenContext())
            {
                c.CRD_ItemBarcodes.Add(viewModel.varyant);
                if (!c.SaveContextWithException()) return;
            }

            DataLayer.V_AllItems.Add(new V_AllItems()
            {
                ID = viewModel.varyant.UrunID.convInt(),
                Aciklama = viewModel.varyant.Aciklama,
                Active = true,
                UnitPrice = viewModel.varyant.Fiyat,
                Miktar = (viewModel.varyant.Miktar).convDouble(),
                UnitID_ = viewModel.item.UnitID_,
                Name = viewModel.item.Name
            });
            BindingContext = new StoklarViewModel() { varyantlar = viewModel.varyantlar, varyant = new Models.CRD_ItemBarcodes(), item = viewModel.item };


        }
        void RebindSatirlar()
        {
            try
            {
                ListViewSatirlar.ItemsSource = new List<CRD_ItemBarcodes>(viewModel.varyantlar.OrderByDescending(s => s.ID));
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
                    EntryBarcode.Text = result.Text;
                });
            };
            await Navigation.PushAsync(scanPage);
        }

        private void Satir_Clicked(object sender, EventArgs e)
        {
            var mi = sender as StackLayout;
            CRD_ItemBarcodes l = (CRD_ItemBarcodes)((TapGestureRecognizer)mi.GestureRecognizers.First()).CommandParameter;

            BindingContext = new StoklarViewModel() { varyantlar = viewModel.varyantlar, varyant = l, item = viewModel.item };
        }
    }
}