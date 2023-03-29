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
    public partial class FiyatDegisiklikSatiri : ContentPage
    {
        public int FisID = 0;
        public bool edit = false, EtiketBasimEmri = false;
        public StokFisleriViewModel viewModel
        {
            get { return (StokFisleriViewModel)BindingContext; }
            set { BindingContext = value; }
        }
        public FiyatDegisiklikSatiri()
        {
            InitializeComponent();
            this.Appearing += StokSatiriEkle_Appearing;

        }

        private void StokSatiriEkle_Appearing(object sender, EventArgs e)
        {

            SatirEntrySeriNo.Focus();
            BtnKaydet.IsEnabled = false;

            if (viewModel.EtiketBasimEmirleri.ProductID == 0) BtnKaydet.IsEnabled = false;
            else
            {
                BtnKaydet.IsEnabled = true;

            }
            if (EtiketBasimEmri)
            {
                Fiyatlar.IsVisible = false;
            }
        }
        bool barkodOkundu = false;
        private void SatirEntrySeriNo_Unfocused(object sender, FocusEventArgs e)
        {
            if (SatirEntrySeriNo.Text + "" == "") return;
            List<V_AllItems> itm = DataLayer.V_AllItems.AsEnumerable().Where(x => x.Barcode == SatirEntrySeriNo.Text).ToList();
            if (itm.Count() == 0)
                appSettings.UyariGoster("Ürün bulunamadı...");
            else
            {
                SatirPickerUrun.SelectedItem = itm.FirstOrDefault();
                if (SatirEntryAmount.Value.convInt() == 0)
                    SatirEntryAmount.Value = 1;
                barkodOkundu = true;
            }
        }

        private void GoldenEntryProductPicker_TextChanged(object sender, EventArgs e)
        {
            Controls.GoldenEntryProductPicker p = sender as Controls.GoldenEntryProductPicker;
            if (p.SelectedItem.ID == 0) return;
            viewModel.EtiketBasimEmirleri.EskiFiyat = p.SelectedItem.UnitPrice;
            SatirEntrySeriNo.Text = p.SelectedItem.Barcode;
            SatirEntryUnitPrice1.Value = p.SelectedItem.UnitPrice.convDecimal();
            SatirEntryUnitPrice2.Value = p.SelectedItem.UnitPrice2.convDecimal();
            SatirEntryUnitPrice3.Value = p.SelectedItem.UnitPrice3.convDecimal();
            LabelEskiFiyat.Text = p.SelectedItem.UnitPrice + "";
            BtnKaydet.IsEnabled = true;
            if (SatirEntryAmount.Value.convInt() == 0)
                SatirEntryAmount.Value = 1;

            SatirEntryAmount.Focus();

            barkodOkundu = false;
        }





        private async void SatirKaydet_Clicked(object sender, EventArgs e)
        {

            if (DataLayer.V_AllItems.Where(s => s.ID == viewModel.EtiketBasimEmirleri.ProductID).Count() > 1 && viewModel.EtiketBasim.TRN_EtiketBasimEmirleri.Where(s => s.ProductID == viewModel.EtiketBasimEmirleri.ProductID).Count() == 1)
            {
                if (await appSettings.Onay("Bu ürüne ait farklı varyantlar mevcut. Onlarında fiyatları güncellensin mi?"))
                {
                    foreach (var i in DataLayer.V_AllItems.Where(s => s.ID == viewModel.EtiketBasimEmirleri.ProductID && s.Barcode != viewModel.EtiketBasimEmirleri.ProductID_.Barcode))
                    {
                        viewModel.EtiketBasimEmirleri = viewModel.EtiketBasimEmirleri as TRN_EtiketBasimEmirleri;
                        viewModel.EtiketBasimEmirleri.Barkod = i.Barcode;
                        viewModel.EtiketBasim.TRN_EtiketBasimEmirleri.Add(viewModel.EtiketBasimEmirleri);
                    }
                }
            }


            viewModel.EtiketBasimEmirleri = new TRN_EtiketBasimEmirleri();
            viewModel.EtiketBasimEmirleri.Tarih = DateTime.Now; //Sıralama için
            viewModel.EtiketBasim.TRN_EtiketBasimEmirleri.Add(viewModel.EtiketBasimEmirleri);
            BindingContext = new StokFisleriViewModel() { EtiketBasim = viewModel.EtiketBasim, EtiketBasimEmirleri = viewModel.EtiketBasimEmirleri };
            SatirEntrySeriNo.Focus();
            BtnKaydet.IsEnabled = false;
            RebindSatirlar();

        }
        void RebindSatirlar()
        {
            try
            {
                ListViewSatirlar.ItemsSource = new List<TRN_EtiketBasimEmirleri>(viewModel.EtiketBasim.TRN_EtiketBasimEmirleri.Where(s => s.ProductID_ != null).Where(s => s.ProductID > 0).OrderByDescending(s => s.Tarih).ThenByDescending(s => s.ID));
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
                    SatirEntrySeriNo_Unfocused(null, null);
                });
            };
            await Navigation.PushAsync(scanPage);
        }



        private void Satir_Clicked(object sender, EventArgs e)
        {
            var mi = sender as MenuItem;
            TRN_EtiketBasimEmirleri l = (TRN_EtiketBasimEmirleri)mi.CommandParameter;

            viewModel.EtiketBasimEmirleri = l;
            BindingContext = new StokFisleriViewModel() { EtiketBasim = viewModel.EtiketBasim, EtiketBasimEmirleri = l };
            RebindSatirlar();
        }

        private async void Sil_Clicked(object sender, EventArgs e)
        {
            var mi = sender as MenuItem;
            TRN_EtiketBasimEmirleri l = (TRN_EtiketBasimEmirleri)mi.CommandParameter;

            if (await appSettings.Onay())
            {
                viewModel.EtiketBasim.TRN_EtiketBasimEmirleri.Remove(l);

            }
            RebindSatirlar();
        }

        private void SatirEntryUnitPrice1_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void SatirEntryUnitPrice1_TezxtChanged(object sender, TextChangedEventArgs e)
        {
            var entry = (Entry)sender;

            var amt = e.NewTextValue;



            if (decimal.TryParse(amt, out decimal num))
            {
                // Init our number

                entry.Text = num.ToString("n2");
            }
            else
            {
                entry.Text = e.OldTextValue;
            }
        }

        private void SatirEntryAmount_Completed(object sender, EventArgs e)
        {
            SatirEntryUnitPrice1.Focus();
        }

        private void SatirEntryUnitPrice1_Completed(object sender, EventArgs e)
        {
            SatirEntryUnitPrice2.Focus();
        }

        private void SatirEntryUnitPrice2_Completed(object sender, EventArgs e)
        {
            SatirEntryUnitPrice3.Focus();
        }

        private void SatirEntryUnitPrice3_Completed(object sender, EventArgs e)
        {
            SatirKaydet_Clicked(null, null);
        }

        private async void Ara_Clicked(object sender, EventArgs e)
        {
            Stoklar fm = new Stoklar();
            fm.ItemSelected += (s2, e2) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Navigation.PopAsync();
                    SatirPickerUrun.SelectedItem = fm.item;
                });
            };
            await Navigation.PushAsync(fm);
        }
    }
}