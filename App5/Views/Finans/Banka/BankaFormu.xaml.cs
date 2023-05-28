using GoldenMobileX.ViewModels;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using GoldenMobileX.Models;
using System.Collections.Generic;
using DevExpress.XamarinForms.Editors;
using Newtonsoft.Json;

namespace GoldenMobileX.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BankaFormu : ContentPage
    {
  
        public FinansViewModel viewModel
        {
            get { return (FinansViewModel)BindingContext; }
            set { BindingContext = value; }
        }
        public BankaFormu()
        {

            InitializeComponent();

            Appearing += BankaFormu_Appearing;

        }

        private void BankaFormu_Appearing(object sender, EventArgs e)
        {
            PickerType_SelectionChanged(null, null);
        }

        private void BtnKaydet_Clicked(object sender, EventArgs e)
        {
            if (viewModel.hareket.BankaHesapID > 0) { viewModel.hareket.BankaID = viewModel.hareket.BankaHesapID_.BankaID; }
            else
            {
                appSettings.UyariGoster("Banka hesabı seçiniz..");
                return;
            }
            if (viewModel.hareket.KarsiBankaHesapID > 0) { viewModel.hareket.KarsiBankaID = viewModel.hareket.KarsiBankaHesapID_.BankaID; }
            if (viewModel.hareket.DovizKodu == null) { viewModel.hareket.DovizKodu = viewModel.hareket.BankaHesapID_.CurrencyID; }
            if (viewModel.hareket.DovizKuru == null) { viewModel.hareket.DovizKuru = appSettings.KurCevir(1, viewModel.hareket.BankaHesapID_.CurrencyID.convInt(), viewModel.hareket.Tarih.convDateTime(), 1); }
            if (DataLayer.IsOfflineAlert) return;
            using (GoldenContext c = new GoldenContext())
            {
                if (viewModel.hareket.Aciklama.Length > 10)
                {
                    if (c.TRN_BankaHareketleri.Where(s => s.Tutar == viewModel.hareket.Tutar && s.Tarih == viewModel.hareket.Tarih && s.Aciklama.StartsWith(viewModel.hareket.Aciklama)).Count() > 0)
                    {
                        appSettings.UyariGoster("Bu işlem daha önce girilmiş.");
                        return;
                    }
                }

                if (viewModel.hareket.ID > 0)
                    c.TRN_BankaHareketleri.Update(viewModel.hareket);
                else
                    c.TRN_BankaHareketleri.Add(viewModel.hareket);

                string pattern = GoldenAI.getPattern(viewModel.hareket.Aciklama);
                var ptrns = c.AI_Patterns.Where(s => s.Pattern == pattern)?.FirstOrDefault();
                if (ptrns == null)
                {
                   
                    AI_Patterns p = new AI_Patterns();
                    p.Pattern = pattern;
                    p.LastString = viewModel.hareket.Aciklama;
                    p.XmlInfo = JsonConvert.SerializeObject(viewModel.hareket);
                    c.AI_Patterns.Add(p);
                }
                else
                {
                    ptrns.LastString = viewModel.hareket.Aciklama;
                    ptrns.XmlInfo = JsonConvert.SerializeObject(viewModel.hareket);
                    c.AI_Patterns.Update(ptrns);
                }
                if (!c.SaveContextWithException()) return;

            }
            Navigation.PopAsync();

        }
        public void Isle(string str)
        {
            if (viewModel.hareket.Tutar.convDouble() == 0)
            {
                List<(string, decimal, int)> moneys = GoldenAI.Tools.FindMoneys(str);
                if (moneys.Count > 0)
                {
                    SatirEntrytutar.Value = moneys[0].Item2.convDecimal();
                    GoldenContext c = new GoldenContext();
                    string pattern = GoldenAI.getPattern(str);
                    AI_Patterns p = c.AI_Patterns.Where(s => s.Pattern.StartsWith(pattern))?.FirstOrDefault();
                    if (p != null)
                    {
                        try
                        {
                            TRN_BankaHareketleri h = JsonConvert.DeserializeObject<TRN_BankaHareketleri>(p.XmlInfo);
                            if (h.BankaHesapID_ != null) eBanka.SelectedItem = h.BankaHesapID_;
                            if (h.KarsiBankaHesapID_ != null)
                            {
                                eKarsiBanka.SelectedItem = h.KarsiBankaHesapID_;
                                eKarsiBanka.IsVisible = true;
                            }
                            if (h.CariID_ != null) ECari.SelectedItem = h.CariID_;
                            if (h.TurKodu != null) PickerType.SelectedValue = h.TurKodu;
                        }
                        catch { }
                    }
                    else
                    {

                    }
                }
            }
            if (viewModel.hareket.Tarih == null)
            {
                List<(string, DateTime, int)> dates = GoldenAI.Tools.FindDates(str);
                if (dates.Count > 0)
                    FisDatePickerTarih.Date = dates[0].Item2.convDateTime();
            }
        }
        private void EntryName_Completed(object sender, EventArgs e)
        {
            Isle((sender as Editor).Text);
        }

        private void PickerType_SelectionChanged(object sender, EventArgs e)
        {
            ECari.IsVisible = true;
            eKarsiBanka.IsVisible = true;
            if (viewModel.hareket.TurKodu == 3)
            {
                ECari.IsVisible = false;
                eKarsiBanka.PlaceholderText = "Paranın yatıralacağı banka";
                eBanka.PlaceholderText = "Paranın çekileceği banka";
            }
            else eKarsiBanka.IsVisible = false;
        }
    }
}