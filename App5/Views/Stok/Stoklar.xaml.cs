using GoldenMobileX.Models;
using GoldenMobileX.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Xamarin.Forms;

namespace GoldenMobileX.Views
{
    public partial class Stoklar : ContentPage
    {

        public V_AllItems item;
        public EventHandler ItemSelected;

        StoklarViewModel viewModel
        {
            get { return (StoklarViewModel)BindingContext; }
        }

        public Stoklar()
        {
            InitializeComponent();
            Rebind("");
            Appearing += Stoklar_Appearing;
        }

        private void Stoklar_Appearing(object sender, EventArgs e)
        {
            EntryAra.Focus();
        }

        async void Rebind(string search)
        {
            System.Globalization.CultureInfo c = new System.Globalization.CultureInfo("tr-TR");
            List<V_AllItems> newlist = new List<V_AllItems>();
            if (search == "")
            {
                newlist = DataLayer.V_AllItems.OrderBy(x => x.ID).ToList();
            }
            else
            {
                var searchwords = search.ToLower(c).Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).ToList();
                newlist = DataLayer.V_AllItems.Where(x => searchwords.All(t => (x.Name + " " + x.Barcode).ToLower(c).Split(' ').Any(s => s.Contains(t)))).OrderBy(x => x.ID).ToList();
            }
            ItemsListview.ItemsSource = newlist;

        }







        private void YeniEkle_Clicked(object sender, EventArgs e)
        {
            StokKarti fm = new StokKarti();
            Navigation.PushAsync(fm);
            fm.Disappearing += Fm_Disappearing;
        }

        private void Fm_Disappearing(object sender, EventArgs e)
        {
            Rebind("");
        }





        private void Yenile_Clicked(object sender, EventArgs e)
        {
            EntryAra.Text = "";
            if (!DataLayer.IsOfflineAlert)
            {
                using (GoldenContext c = new GoldenContext())
                {
                    DataLayer.V_AllItems = c.V_AllItems.Where(s => s.Active == true).Select(s => s).ToList();
                }
            }
            Rebind("");
        }

        private void Duzenle_Clicked(object sender, EventArgs e)
        {
            StokKarti fm = new StokKarti();
            var mi = sender as MenuItem;
            fm.viewModel = new StoklarViewModel() { item = (V_AllItems)mi.CommandParameter, items = viewModel.items };
            fm.Disappearing += Fm_Disappearing;
            Navigation.PushAsync(fm);

        }



        private void Sil_Clicked(object sender, EventArgs e)
        {
            var mi = sender as MenuItem;

            viewModel.items.Remove((V_AllItems)mi.CommandParameter);

        }



        private void StokListesi_Tapped(object sender, EventArgs e)
        {
            var mi = sender as StackLayout;
            V_AllItems i = (V_AllItems)((TapGestureRecognizer)mi.GestureRecognizers.First()).CommandParameter;
            item = i;
            if (ItemSelected != null)
                ItemSelected(sender, e);

        }



        private void ItemsListview_Scrolled(object sender, ScrolledEventArgs e)
        {
            if (e.ScrollY == (sender as ListView).Height)
            {

            }
        }

        private void ButtonAra_Clicked(object sender, TextChangedEventArgs e)
        {

            Rebind(EntryAra.Text);
        }

        private void Hareketler_Clicked(object sender, EventArgs e)
        {
            var mi = sender as MenuItem;
            V_AllItems i = (V_AllItems)mi.CommandParameter;
            StokHareketleri fm = new StokHareketleri();
            fm.ProductID = i.ID;
            Navigation.PushAsync(fm);
        }
    }
}

