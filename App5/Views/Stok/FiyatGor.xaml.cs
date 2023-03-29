using System;
using System.Linq;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using ZXing.Net.Mobile.Forms;

namespace GoldenMobileX.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FiyatGor : ContentPage
    {
        public FiyatGor()
        {
            InitializeComponent();
            Appearing += FiyatGor_Appearing;
        }

        private void FiyatGor_Appearing(object sender, EventArgs e)
        {
            EntryBarcode.Focus();
        }

        void FiyatGetir(string barkod)
        {
            var itm = DataLayer.V_AllItems.Where(s => s.Barcode == barkod);
            if (itm.Count() == 0)
                DisplayAlert("", "Ürün bulunamadı", "Ok");
            else
            {
                DisplayAlert("", itm.First().Name + " " + itm.First().UnitPrice + " ", "Ok");
                EntryBarcode.Text = "";
                EntryBarcode.Focus();
            }
        }
        private void EntryBarkod_Completed(object sender, EventArgs e)
        {
            FiyatGetir((sender as Entry).Text);
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
                    FiyatGetir(result.Text);
                });
            };
            await Navigation.PushAsync(scanPage);

        }
    }
}