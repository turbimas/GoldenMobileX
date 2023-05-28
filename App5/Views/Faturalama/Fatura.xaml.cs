using Android.Content;
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
    public partial class Fatura : ContentPage
    {
        public bool ReadOnly = false;
        public bool newAdd = false;
        GoldenContext c = new GoldenContext();
        public StokFisleriViewModel viewModel
        {
            get { return (StokFisleriViewModel)BindingContext; }
            set { BindingContext = value; }
        }
        public Fatura()
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
        
                Kaydet();

        }
        void Kaydet()
        {
            if (DataLayer.IsOfflineAlert) return;

            if (viewModel.Invoice?.Lines != null)
            {
                viewModel.Invoice.Lines.RemoveAll(s => s.Amount == 0);
            }
 
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
                    viewModel.Invoice.Total = viewModel.Invoice.Lines.Sum(x => x.Total).convDouble(2);
                    if (viewModel.Invoice.ID > 0)
                    {
                        viewModel.Invoice.ModifiedBy = appSettings.User.ID;
                        viewModel.Invoice.ModifiedDate = DateTime.Now;
                        c.TRN_Invoice.Update(viewModel.Invoice);
                    }
                    else
                    {
                        viewModel.Invoice.CreatedBy = appSettings.User.ID;
                        viewModel.Invoice.CreatedDate = DateTime.Now;
                        c.TRN_Invoice.Add(viewModel.Invoice);
                    }
                    c.SaveChanges();
                    foreach (var l in viewModel.Invoice.Lines)
                    {
                        l.InvoiceID = viewModel.Invoice.ID;
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

        bool FisKaydetEnabled()
        {
            bool kaydet = false;

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