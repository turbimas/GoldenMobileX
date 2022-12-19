using GoldenMobileX.ViewModels;
using GoldenMobileX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Globalization;

namespace GoldenMobileX.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CariHesaplar : ContentPage
    {
 
         public EventHandler ItemSelected;
        public CRD_Cari SelectedCari;
        FinansViewModel viewModel
        {
            get { return (FinansViewModel)BindingContext; }
        }

        public CariHesaplar()
        {
            InitializeComponent();

            CariHesaplariGoster();
            BindingContext = new FinansViewModel();
        }
        void CariHesaplariGoster()
        {
            Rebind("");
            
        }

        void Rebind(string ara)
        {
            CultureInfo c = new CultureInfo("tr-TR");
            if (ara == "")
                ListViewCariHesaplar.ItemsSource = DataLayer.Cariler.OrderBy(x => x.Name);
            else
            {
                var searchwords = ara.ToLower(c).Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
     
                ListViewCariHesaplar.ItemsSource = DataLayer.Cariler.Where(x => searchwords.All(t => (x.Name + " " + x.TaxNumber + " " +x.TCKNo).ToLower(c).Split(' ').Any(s => s.Contains(t)))).OrderBy(x => x.Name).ToList();

            }
        }
        async private void Duzenle_Clicked(object sender, EventArgs e)
        {
            var mi = sender as MenuItem;
            viewModel.item = DataLayer.Cariler.Where(x => x.ID == ((CRD_Cari)mi.CommandParameter).ID).First();


            CariHesapKarti fm = new CariHesapKarti();

            fm.viewModel = viewModel;
            fm.Disappearing += Fm_Disappearing;
            await Navigation.PushAsync(fm);

        }

        private void Fm_Disappearing(object sender, EventArgs e)
        {
            ListViewCariHesaplar.ItemsSource = DataLayer.Cariler.OrderBy(x => x.Name);
        }

        async private void YeniCari_Clicked(object sender, EventArgs e)
        {
            CariHesapKarti fm = new CariHesapKarti();
            fm.viewModel = new FinansViewModel() { item = new CRD_Cari() { Active = true } };
            fm.Disappearing += Fm_Disappearing;
            await Navigation.PushAsync(fm);
        }



        private void Yenile_Clicked(object sender, EventArgs e)
        {
            CariHesaplariGoster();
        }


        async void Hareketler_Clicked(object sender, EventArgs e)
        {
            var mi = sender as MenuItem;


            CariHesapHareketleri fm = new CariHesapHareketleri();
            fm.CariID = ((CRD_Cari)mi.CommandParameter).ID;
            fm.Disappearing += (s2, e2) =>
            {
                Device.BeginInvokeOnMainThread(() =>
                {
                    Navigation.PopAsync();

                });
            };
            await Navigation.PushAsync(fm);

        }

 

        private void SearchBar_TextChanged(object sender, TextChangedEventArgs e)
        {
                Rebind(EntryAra.Text);
        }

        private void CariListesi_Tapped(object sender, EventArgs e)
        {
            var mi = sender as StackLayout;
            CRD_Cari i = (CRD_Cari)((TapGestureRecognizer)mi.GestureRecognizers.First()).CommandParameter;
            SelectedCari = i;
            if (ItemSelected != null)
                ItemSelected(sender, e);
        }
    }
}


