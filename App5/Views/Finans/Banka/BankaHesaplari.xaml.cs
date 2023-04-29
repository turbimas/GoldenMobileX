using GoldenMobileX.ViewModels;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using GoldenMobileX.Models;
using System.Collections.Generic;
namespace GoldenMobileX.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BankaHesaplari : ContentPage
    {
        public int BankaID = 0;
        public int BankaHesapID = 0;
        public bool  OnlySelect = false;
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
           
            if(!DataLayer.IsOfflineAlert)
            {
                using (GoldenContext c = new GoldenContext())
                {
                    List<CRD_BankaHesaplari> hesaplar = c.CRD_BankaHesaplari.Where(s => s.BankaID == BankaID && s.Active == true).ToList();
 
                    ListViewHareketler.ItemsSource = hesaplar;
                }
            }
            Title = DataLayer.CRD_Bankalar.Where(s => s.ID == BankaID).FirstOrDefault().BankaAdi;
        }

        private void IlgiliKayit_Clicked(object sender, EventArgs e)
        {

        }

        private void Bankalar_Tapped(object sender, EventArgs e)
        {
            var mi = sender as StackLayout;
            CRD_BankaHesaplari t = (CRD_BankaHesaplari)((TapGestureRecognizer)mi.GestureRecognizers.First()).CommandParameter;

            BankaHesapID = t.ID;
            if (OnlySelect) Navigation.PopAsync();
            else
            {
                BankaHareketleri fm = new BankaHareketleri();
                fm.BankaHesapID = t.ID;
                fm.BankaID = t.BankaID.convInt();

                Navigation.PushAsync(fm);
            }
        }
    }
}