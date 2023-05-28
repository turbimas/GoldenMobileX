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
    public partial class BankaSMSleri : ContentPage
    {
        public EventHandler ItemSelected;
        public CRD_BankaHesaplari SelectedItem;
        public bool OnlySelect = false;
        public FinansViewModel viewModel
        {
            get { return (FinansViewModel)BindingContext; }
            set { BindingContext = value; }
        }
        public BankaSMSleri()
        {
            InitializeComponent();
            Appearing += StokHareketleri_Appearing;
        }
        string txtMeans = "BANKASMS";
        private void StokHareketleri_Appearing(object sender, EventArgs e)
        {
            Rebind();   
        }
        void Rebind()
        {
            if (!DataLayer.IsOfflineAlert)
            {
                using (GoldenContext c = new GoldenContext())
                {
                    List<AI_Dictionary> list = c.AI_Dictionary.Where(s => s.Means == txtMeans).Select(s => s).ToList();
                    try
                    {

                        var smsService = DependencyService.Get<ISmsService>();
                        var smsMessages = smsService.GetRecentSmsMessages(DateTime.Now.AddDays(-3));
                        string str = "";
                        foreach (var m in smsMessages.Select(s=>s.Address).Distinct())
                        {
                            list.Add(new AI_Dictionary() { Means = "", Txt = m });
                        }

                    }
                    catch (Exception ex)
                    {
                        appSettings.UyariGoster("SMS Hatası : " + ex.Message + ex.InnerException?.Message);
                    }

                    ListViewHareketler.ItemsSource = list;
                }

            }
        }

        private void Bankalar_Tapped(object sender, EventArgs e)
        {
            var mi = sender as StackLayout;
            AI_Dictionary t = (AI_Dictionary)((TapGestureRecognizer)mi.GestureRecognizers.First()).CommandParameter;
 if(t != null)
            {
                if (!DataLayer.IsOfflineAlert)
                {
                    using (GoldenContext c = new GoldenContext())
                    {
                        if (t.Means == txtMeans)
                        {
                            c.AI_Dictionary.Remove(t);
                        }
                        else
                        {
                            t.Means = txtMeans;
                            c.AI_Dictionary.Add(t);
                        }
                        c.SaveContextWithException();
                    }
                }
            }
            Rebind();

   
        }

    }
}