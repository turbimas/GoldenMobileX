using GoldenMobileX.Models;
using GoldenMobileX.ViewModels;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoldenMobileX.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class FiyatDegisiklikFisleri : ContentPage
    {

        StokFisleriViewModel viewModel
        {
            get { return (StokFisleriViewModel)BindingContext; }
        }
        public FiyatDegisiklikFisleri()
        {
            InitializeComponent();
            this.BindingContext = new StokFisleriViewModel();
            Rebind();

        }

        void Rebind()
        {
            try
            {
                IsBusy = true;
                viewModel.EtiketBasim_List = DataLayer.WaitingSent.TRN_EtiketBasim;
                this.BindingContext = new StokFisleriViewModel() { EtiketBasim_List = new List<TRN_EtiketBasim>(viewModel.EtiketBasim_List) };
                IsBusy = false;
            }
            catch (Exception ex)
            {
                appSettings.UyariGoster(ex.Message);
                IsBusy = false;
            }
            finally
            {
                IsBusy = false;
            }

        }



        private void Fm_Disappearing(object sender, EventArgs e)
        {
            Rebind();
        }



        private void Filtrele_Clicked(object sender, EventArgs e)
        {
            Rebind();
        }



        private void Duzenle_Clicked(object sender, EventArgs e)
        {
            var mi = sender as SwipeItem;
            TRN_EtiketBasim t = (TRN_EtiketBasim)mi.CommandParameter;
            FiyatDegisiklikFisi fm = new FiyatDegisiklikFisi();

            fm.Disappearing += Fm_Disappearing;



            if ((t.Tarih).convDateTime().Date < DateTime.Now.Date)
            {
                appSettings.UyariGoster("Geçmiş tarihli fişleri değiştiremezsiniz.");
                return;
            }

            viewModel.EtiketBasim = t;
            fm.viewModel = viewModel;

            Navigation.PushAsync(fm);
        }



        private void Sil_Clicked(object sender, EventArgs e)
        {
            var mi = sender as SwipeItem;
            TRN_EtiketBasim t = (TRN_EtiketBasim)mi.CommandParameter;

            DataLayer.WaitingSent.TRN_EtiketBasim.Remove(t);
            DataLayer.WaitingSent.SaveJSON();
            Rebind();
        }
        private void YeniFis_Clicked(object sender, EventArgs e)
        {
            FiyatDegisiklikFisi fm = new FiyatDegisiklikFisi();
            fm.newAdd = true;
            fm.viewModel = new StokFisleriViewModel()
            {
                EtiketBasim = new TRN_EtiketBasim()
                {
                    FisNo = appSettings.GetCounter("EtiketBasim", "TRN_EtiketBasim", "FisNo", "TRN" + DateTime.Now.Year),
                    Tarih = DateTime.Now,
                    Branch = appSettings.UserDefaultBranch,
                    TRN_EtiketBasimEmirleri = new List<TRN_EtiketBasimEmirleri>()
                }
            };
            fm.Disappearing += Fm_Disappearing;

            Navigation.PushAsync(fm);


        }
        private void EtiketBasimEmri_Clicked(object sender, EventArgs e)
        {
            FiyatDegisiklikFisi fm = new FiyatDegisiklikFisi();
            fm.newAdd = true;
            fm.EtiketBasimEmri = true;
            fm.viewModel = new StokFisleriViewModel()
            {
                EtiketBasim = new TRN_EtiketBasim()
                {
                    FisNo = appSettings.GetCounter("EtiketBasim", "TRN_EtiketBasim", "FisNo", "TRN" + DateTime.Now.Year),
                    Tarih = DateTime.Now,
                    Branch = appSettings.UserDefaultBranch,
                    TRN_EtiketBasimEmirleri = new List<TRN_EtiketBasimEmirleri>()
                }
            };
            fm.Disappearing += Fm_Disappearing;

            Navigation.PushAsync(fm);
        }

        private void SunucuyaGonder_Invoked(object sender, EventArgs e)
        {
            var mi = sender as SwipeItem;
            TRN_EtiketBasim t = (TRN_EtiketBasim)mi.CommandParameter;
            DataLayer.TRN_EtiketBasimInsert(t);
            Rebind();
        }
    }
}


