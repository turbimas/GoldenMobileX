using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GoldenMobileX.ViewModels;
using GoldenMobileX.Models;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoldenMobileX.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BankaHesaplari : ContentPage
    {
        public int BankaID = 0;
        public FinansViewModel viewModel
        {
            get { return (FinansViewModel)BindingContext; }
            set { BindingContext = value; }
        }
        public BankaHesaplari()
        {
            InitializeComponent();
            Appearing += StokHareketleri_Appearing;
        }

        private void StokHareketleri_Appearing(object sender, EventArgs e)
        {
            ListViewHareketler.ItemsSource = DataLayer.CRD_BankaHesaplari.Where(s => s.BankaID == BankaID);
            Title = DataLayer.CRD_Bankalar.Where(s => s.ID == BankaID).FirstOrDefault().BankaAdi;
        }

        private void IlgiliKayit_Clicked(object sender, EventArgs e)
        {

        }

        private void Bankalar_Tapped(object sender, EventArgs e)
        {

        }
    }
}