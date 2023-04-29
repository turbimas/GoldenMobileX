using GoldenMobileX.ViewModels;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using GoldenMobileX.Models;
using System.Collections.Generic;
using static Android.Resource;

namespace GoldenMobileX.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BankaHareketleri : ContentPage
    {
        public int BankaID = 0;
        public int BankaHesapID = 0;
        GoldenContext c = new GoldenContext();
        public FinansViewModel viewModel
        {
            get { return (FinansViewModel)BindingContext; }
            set { BindingContext = value; }
        }
        public BankaHareketleri()
        {
            InitializeComponent();
            this.Appearing += BankaHareketleri_Appearing;
        }

        private void BankaHareketleri_Appearing(object sender, EventArgs e)
        {
            if (!DataLayer.IsOfflineAlert)
            {

                    List<TRN_BankaHareketleri> hesaplar = c.TRN_BankaHareketleri.Where(s => s.BankaHesapID == BankaHesapID).Select(s => s).OrderByDescending(s=>s.Tarih)?.ToList();

                    ListViewHareketler.ItemsSource = hesaplar;
            }
            Title = DataLayer.CRD_BankaHesaplari.Where(s => s.ID == BankaHesapID).FirstOrDefault().HesapAdi;
        }

        private void Hareket_Tapped(object sender, EventArgs e)
        {
            if (DataLayer.IsOfflineAlert) return;
            var mi = sender as StackLayout;
            TRN_BankaHareketleri t = (TRN_BankaHareketleri)((TapGestureRecognizer)mi.GestureRecognizers.First()).CommandParameter;
            BankaFormu fm = new BankaFormu();
            fm.viewModel = new FinansViewModel() { hareket = c.TRN_BankaHareketleri.Where(s => s.ID == t.ID).Select(s => s).FirstOrDefault() };
  
      
            Navigation.PushAsync(fm);
        }

 

        private void Yenile_Clicked(object sender, EventArgs e)
        {
            BankaHareketleri_Appearing(null, null);
        }
        /*
         * 
         * 
         * TRN_BankaHareketleri	TurKodu	0	Gelen Havale
TRN_BankaHareketleri	TurKodu	1	Gönderilen Havale
TRN_BankaHareketleri	TurKodu	2	Para Yatırma
TRN_BankaHareketleri	TurKodu	3	Para Çekme
TRN_BankaHareketleri	TurKodu	4	Kredi Taksiti Ödeme
TRN_BankaHareketleri	TurKodu	5	Kredi Taksit Girişi (Borçlanma)
TRN_BankaHareketleri	TurKodu	6	Masraf / Gider
TRN_BankaHareketleri	TurKodu	7	Faiz Geliri
TRN_BankaHareketleri	TurKodu	8	Kredi Kartı ile Tahsilat
TRN_BankaHareketleri	TurKodu	9	Virman
TRN_BankaHareketleri	TurKodu	13	Çek Senet Bankaya Tahsile Gönder
TRN_BankaHareketleri	TurKodu	14	Çek Senet Bankaya Teminat
TRN_BankaHareketleri	TurKodu	15	Çek Ödeme
TRN_BankaHareketleri	TurKodu	130	Çek Senet Bankadan Portfoye İade
TRN_BankaHareketleri	TurKodu	131	Çek Senet Bankadan Tahsil
TRN_BankaHareketleri	TurKodu	132	Çek Senet Bankada Karşılıksız
TRN_BankaHareketleri	TurKodu	100	Kapanış Fişi
TRN_BankaHareketleri	TurKodu	101	Açılış Fişi
        */
        private void GelenHavale_Clicked(object sender, EventArgs e)
        {
            BankaFormu fm = new BankaFormu();
            fm.viewModel = new FinansViewModel() { hareket = new TRN_BankaHareketleri() { BankaID = BankaID, BankaHesapID = BankaHesapID, TurKodu=0 } };
            fm.Disappearing += Fm_Disappearing;
            Navigation.PushAsync(fm);
        }

        private void GonderilenHavale_Clicked(object sender, EventArgs e)
        {
            BankaFormu fm = new BankaFormu();
            fm.viewModel = new FinansViewModel() { hareket = new TRN_BankaHareketleri() { BankaID = BankaID, BankaHesapID = BankaHesapID, TurKodu = 1} };
            fm.Disappearing += Fm_Disappearing;
            Navigation.PushAsync(fm);
        }

        private void BaskaHesabaParaGonder_Clicked(object sender, EventArgs e)
        {
            BankaFormu fm = new BankaFormu();
            fm.viewModel = new FinansViewModel() { hareket = new TRN_BankaHareketleri() { BankaID = BankaID, BankaHesapID = BankaHesapID, TurKodu = 3 } };
            fm.Disappearing += Fm_Disappearing;
            Navigation.PushAsync(fm);
        }

        private void Fm_Disappearing(object sender, EventArgs e)
        {
            BankaHareketleri_Appearing(null, null);
        }
    }
}