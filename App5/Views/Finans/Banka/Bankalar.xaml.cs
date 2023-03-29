using GoldenMobileX.Models;
using GoldenMobileX.ViewModels;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
namespace GoldenMobileX.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Bankalar : ContentPage
    {
        public FinansViewModel viewModel
        {
            get { return (FinansViewModel)BindingContext; }
            set { BindingContext = value; }
        }
        public Bankalar()
        {
            InitializeComponent();
            Appearing += StokHareketleri_Appearing;
        }

        private void StokHareketleri_Appearing(object sender, EventArgs e)
        {
            foreach (var b in DataLayer.CRD_Bankalar)
            {
                b.Bakiye = new Toplam() { Value = DataLayer.CRD_BankaHesaplari.Where(s => s.BankaID == b.ID).Sum(s => s.Bakiye) };

            }
            ListViewHareketler.ItemsSource = DataLayer.CRD_Bankalar.Where(x=>x.Active==true);
        }

        private void IlgiliKayit_Clicked(object sender, EventArgs e)
        {

        }

        private void Bankalar_Tapped(object sender, EventArgs e)
        {
            var mi = sender as StackLayout;
            CRD_Bankalar t = (CRD_Bankalar)((TapGestureRecognizer)mi.GestureRecognizers.First()).CommandParameter;
            BankaHesaplari fm = new BankaHesaplari();
            fm.BankaID = t.ID;

            Navigation.PushAsync(fm);
        }
    }
}