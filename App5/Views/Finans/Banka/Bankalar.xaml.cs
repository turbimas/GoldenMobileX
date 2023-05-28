using GoldenMobileX.Models;
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
    public partial class Bankalar : ContentPage
    {
        public EventHandler ItemSelected;
        public CRD_BankaHesaplari SelectedItem;
        public bool OnlySelect = false;
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
            if (!DataLayer.IsOfflineAlert)
            {
                using (GoldenContext c = new GoldenContext())
                {
                    TRN_DailyExchange usd = c.TRN_DailyExchange.Where(s => s.CurrencyID == 1).OrderByDescending(s => s.Date).FirstOrDefault();
                    TRN_DailyExchange eur = c.TRN_DailyExchange.Where(s => s.CurrencyID == 20).OrderByDescending(s => s.Date).FirstOrDefault();
                    foreach (var b in DataLayer.CRD_Bankalar)
                    {
                        b.Bakiye = new Toplam() { Value = c.CRD_BankaHesaplari.Where(s => s.BankaID == b.ID).Select(s => s).AsEnumerable()?.ToList().Sum(s => ((s.HesapBakiye).convDouble() * s.CurrencyID == 1 ? usd.Rate1 : 1) * (s.CurrencyID == 20 ? eur.Rate1 : 1)).convDecimal() };

                    }
                }
                ListViewHareketler.ItemsSource = DataLayer.CRD_Bankalar.Where(x => x.Active == true);
            }
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
            fm.OnlySelect = OnlySelect;
            fm.Disappearing += (s2, e2) =>
            {
                SelectedItem = DataLayer.CRD_BankaHesaplari.Where(s => s.ID == fm.BankaHesapID).FirstOrDefault();
                if (ItemSelected != null)
                    ItemSelected(sender, e);
            };

            Navigation.PushAsync(fm);
        }

        private void SMS_Clicked(object sender, EventArgs e)
        {
            BankaSMSleri fm = new BankaSMSleri();
            Navigation.PushAsync(fm);
        }


    }
}