using GoldenMobileX.Models;
using GoldenMobileX.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoldenMobileX.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Siparis : ContentPage
    {
        public bool ReadOnly = false;
        public bool newAdd = false;
        public OrdersViewModel viewModel
        {
            get { return (OrdersViewModel)BindingContext; }
            set { BindingContext = value; }
        }
        public Siparis()
        {
            InitializeComponent();
            this.Appearing += Siparis_Appearing;
            this.Title = "";
        }



        private void Siparis_Appearing(object sender, EventArgs e)
        {
            if (ReadOnly)
            {
                ToolbarItems.Remove(BtnSatirEkle);

                ToolbarItems.Remove(BtnKaydet);
                SiparisStack.IsEnabled = false;
            }
            RebindSatir();
            FisKaydetEnabled();
            if (this.Title == "") this.Title = viewModel.Order.OrderType_.Name;
            if (viewModel.Order.OrderType_.Code == 1) CariList.IsVisible = false;

        }

        private void FisKaydet_Clicked(object sender, EventArgs e)
        {
            viewModel.Order.Status_ = DataLayer.x_types_SatisSiparisleriDurum.Where(t => t.Code == 0).First();
            if (viewModel.Order.ID > 0)
            {
                viewModel.Order.ModifiedBy = appSettings.User.ID;
                viewModel.Order.ModifiedDate = DateTime.Now;
            }
            else
            {
                viewModel.Order.CreatedBy = appSettings.User.ID;
                viewModel.Order.CreatedDate = DateTime.Now;
            }

            viewModel.Order.Total = viewModel.Order.Lines.Sum(x => x.Total).convDouble();
            if (newAdd)
                DataLayer.WaitingSent.tRN_Orders.Add(viewModel.Order);
            DataLayer.WaitingSent.SaveJSON();
            Navigation.PopAsync();
        }
        void FisKaydetEnabled()
        {
            bool kaydet = false;
            if (viewModel.Order.OrderType_ != null && viewModel.Order.CurrencyID_ != null)
            {
                kaydet = true;
            }

            BtnKaydet.IsEnabled = kaydet;
            BtnSatirEkle.IsEnabled = kaydet;

        }
        private void SatirEkle_Clicked(object sender, EventArgs e)
        {
            if (viewModel.Order.OrderType_ == null)
            {
                appSettings.UyariGoster("Lütfen önce fiş türünü seçiniz..");
                return;
            }
            SiparisSatiriEkle fm = new SiparisSatiriEkle();
            viewModel.Line = new TRN_OrderLines();
            fm.viewModel = new OrdersViewModel() { Order = viewModel.Order, Line = viewModel.Line };

            Navigation.PushAsync(fm);
        }
        private void Satir_Clicked(object sender, EventArgs e)
        {
            var mi = sender as StackLayout;
            TRN_OrderLines l = (TRN_OrderLines)((TapGestureRecognizer)mi.GestureRecognizers.First()).CommandParameter;

            if (viewModel.Order.OrderType_ == null)
            {
                appSettings.UyariGoster("Lütfen önce fiş türünü seçiniz..");
                return;
            }
            SiparisSatiriEkle fm = new SiparisSatiriEkle();
            viewModel.Line = l;

            fm.viewModel = viewModel;
            Navigation.PushAsync(fm);

        }
        void RebindSatir()
        {
            if (viewModel.Order?.Lines != null)
            {
                viewModel.Order.Lines.RemoveAll(s => s.ProductID_ == null);
                ListViewSatirlar.ItemsSource = new List<TRN_OrderLines>(viewModel.Order.Lines).OrderByDescending(s => s.ID);
            }
        }

        private void FisPicker_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}