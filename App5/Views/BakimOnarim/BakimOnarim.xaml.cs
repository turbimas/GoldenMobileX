using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using GoldenMobileX.ViewModels;
using GoldenMobileX.Models;
namespace GoldenMobileX.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BakimOnarim : ContentPage
    {
        public BakimOnarimViewModel viewModel
        {
            get; set;
        }
        public BakimOnarim()
        {
            InitializeComponent();
            viewModel = new BakimOnarimViewModel();
            this.BindingContext = viewModel;
            Rebind();
        }
        void Rebind()
        {
            if (DataLayer.IsOfflineAlert) return;

            using (GoldenContext c = new GoldenContext())
            {
                try
                {
                    viewModel = new BakimOnarimViewModel() { BakimOnarimListesi = c.Kalite_KalibrasyonGirisi.Select(s => s).OrderByDescending(s => s.ID).ToList() };
                }
                catch(Exception ex)
                {
                    appSettings.UyariGoster(ex.Message + " " + ex.InnerException?.Message);
                }
            }
        }
        private void Duzenle_Clicked(object sender, EventArgs e)
        {
            BakimOnarimKarti fm = new BakimOnarimKarti();
            fm.viewModel = viewModel;
            fm.Disappearing += Fm_Disappearing;
            Navigation.PushAsync(fm);
        }

        private void Fm_Disappearing(object sender, EventArgs e)
        {
            Rebind();
        }

        private void Sil_Clicked(object sender, EventArgs e)
        {

        }

        private void YeniFis_Clicked(object sender, EventArgs e)
        {

        }
    }
}