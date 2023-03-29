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
    public partial class FiyatDegisiklikFisi : ContentPage
    {
        public bool EtiketBasimEmri = false;
        public bool newAdd = false;
        public StokFisleriViewModel viewModel
        {
            get { return (StokFisleriViewModel)BindingContext; }
            set { BindingContext = value; }
        }
        public FiyatDegisiklikFisi()
        {
            InitializeComponent();
            this.Appearing += StokFisi_Appearing;

        }


        private void StokFisi_Appearing(object sender, EventArgs e)
        {
            RebindSatir();
            FisKaydetEnabled();

        }

        private void FisKaydet_Clicked(object sender, EventArgs e)
        {

            if (viewModel.EtiketBasim.ID > 0)
            {
                viewModel.EtiketBasim.ModifiedBy = appSettings.User.ID;
                viewModel.EtiketBasim.ModifiedDate = DateTime.Now;
            }
            else
            {
                viewModel.EtiketBasim.CreatedBy = appSettings.User.ID;
                viewModel.EtiketBasim.CreatedDate = DateTime.Now;
            }
            if (newAdd)
            {
                DataLayer.WaitingSent.TRN_EtiketBasim.Add(viewModel.EtiketBasim);
            }
            DataLayer.WaitingSent.SaveJSON();

            Navigation.PopAsync();

        }
        void FisKaydetEnabled()
        {
            bool kaydet = true;

            BtnKaydet.IsEnabled = kaydet;
            BtnSatirEkle.IsEnabled = kaydet;

        }
        private void SatirEkle_Clicked(object sender, EventArgs e)
        {

            FiyatDegisiklikSatiri fm = new FiyatDegisiklikSatiri();
            viewModel.EtiketBasimEmirleri = new TRN_EtiketBasimEmirleri();
            viewModel.EtiketBasimEmirleri.Tarih = DateTime.Now; //Sıralama için
            viewModel.EtiketBasim.TRN_EtiketBasimEmirleri.Add(viewModel.EtiketBasimEmirleri);
            fm.viewModel = new StokFisleriViewModel() { EtiketBasim = viewModel.EtiketBasim, EtiketBasimEmirleri = viewModel.EtiketBasimEmirleri };
            fm.EtiketBasimEmri = EtiketBasimEmri;
            Navigation.PushAsync(fm);
        }
        private void Satir_Clicked(object sender, EventArgs e)
        {
            var mi = sender as StackLayout;
            TRN_EtiketBasimEmirleri l = (TRN_EtiketBasimEmirleri)((TapGestureRecognizer)mi.GestureRecognizers.First()).CommandParameter;

            FiyatDegisiklikSatiri fm = new FiyatDegisiklikSatiri();
            viewModel.EtiketBasimEmirleri = l;
            fm.EtiketBasimEmri = EtiketBasimEmri;
            fm.viewModel = viewModel;
            Navigation.PushAsync(fm);

        }
        void RebindSatir()
        {
            if (viewModel.EtiketBasim?.TRN_EtiketBasimEmirleri != null)
            {
                viewModel.EtiketBasim.TRN_EtiketBasimEmirleri.RemoveAll(s => s.ProductID_ == null);
                ListViewSatirlar.ItemsSource = new List<TRN_EtiketBasimEmirleri>(viewModel.EtiketBasim.TRN_EtiketBasimEmirleri.OrderByDescending(s => s.Tarih).ThenByDescending(s => s.ID));
            }
        }


    }
}