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
    public partial class StokKarti : ContentPage
    {
        public int ID = 0;
        public StoklarViewModel viewModel
        {
            get { return (StoklarViewModel)BindingContext; }
            set { BindingContext = value; }
        }
        public StokKarti()
        {
            if (viewModel == null)
            {

                viewModel = new StoklarViewModel()
                {
                    item = new V_AllItems()
                    {
                        UnitID_ = DataLayer.L_Units.Where(s => s.ID == 11).FirstOrDefault(),
                        Type_ = DataLayer.x_types_stokkarti.Where(s => s.Code == 1).FirstOrDefault(),
                        TaxRate = 18,
                        Active = true
                    }
                    ,
                    files = new List<TRN_Files>()

                };
            }
            InitializeComponent();
        }

        private void BtnKaydet_Clicked(object sender, EventArgs e)
        {

            if (!DataLayer.IsOnline)
            {
                appSettings.UyariGoster("Stok kartı düzenlemek için online olmalısınız.");
                return;
            }


            if (DataLayer.V_AllItems.Where(s => s.Barcode == viewModel.item.Barcode && s != viewModel.item).Count() > 0)
            {
                appSettings.UyariGoster("Bu barkod kullanımda");
                return;
            }
            CRD_Items itm = new CRD_Items()
            {
                ID = viewModel.item.ID,
                Name = viewModel.item.Name,
                Active = viewModel.item.Active,
                AlisFiyati = viewModel.item.AlisFiyati,
                Barcode = viewModel.item.Barcode,
                Code = viewModel.item.Code,
                UnitPrice = viewModel.item.UnitPrice,
                TaxRate = viewModel.item.TaxRate,
                UnitID = viewModel.item.UnitID,
                Type = viewModel.item.Type
            };
            using (GoldenContext c = new GoldenContext())
            {
                c.CRD_Items.Add(itm);
                if (!c.SaveContextWithException()) return;
                viewModel.item.ID = itm.ID;
                DataLayer.V_AllItems.Add(viewModel.item);
            }

            if (viewModel.files != null)
                foreach (var f in viewModel.files)
                {
                    if (f.ID == 0)
                        if (!DataLayer.IsOfflineAlert)
                        {
                            using (GoldenContext c = new GoldenContext())
                            {
                                c.TRN_Files.Add(f);
                                if (!c.SaveContextWithException()) return;
                            }
                        }
                }
            Navigation.PopAsync();
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

        private void BtnResimler_Clicked(object sender, EventArgs e)
        {
            StokResimleri fm = new StokResimleri();
            fm.viewModel = viewModel;
            fm.Appearing += Fm_Appearing;
            Navigation.PushAsync(fm);
        }

        private void Fm_Appearing(object sender, EventArgs e)
        {
            (sender as StokResimleri).Rebind();
        }

        private void BtnVaryantlar_Clicked(object sender, EventArgs e)
        {
            if (DataLayer.IsOfflineAlert) return;
            StokVaryantlar fm = new StokVaryantlar();
            fm.title = viewModel.item.Name;
            fm.viewModel = new StoklarViewModel() { item = viewModel.item, varyantlar = DataLayer.CRD_ItemBarcodes(viewModel.item.ID) };
            Navigation.PushAsync(fm);
        }

        private void BtnYeniBarkod_Clicked(object sender, EventArgs e)
        {
            EntryBarcode.Text = DataLayer.GetBarkod("AnaBarkod", "63", 6);
        }

        private async void PasifYap_Clicked(object sender, EventArgs e)
        {
            if (await appSettings.Onay("Bu kartı pasif yaparsanız sadece ERP programından tekrar aktif yapabilirsiniz. Devam Etmek İstiyor musunuz?"))
                viewModel.item.Active = false;
        }
    }
}