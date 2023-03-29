using GoldenMobileX.Models;
using GoldenMobileX.ViewModels;
using System;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoldenMobileX.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CariHesapKarti : ContentPage
    {
        public int ID = 0;

        public FinansViewModel viewModel
        {
            get { return (FinansViewModel)BindingContext; }
            set { BindingContext = value; }
        }
        public CariHesapKarti()
        {
            InitializeComponent();
            BindingContext = new FinansViewModel();
            if (ID > 0)
            {
                BindingContext = new FinansViewModel() { item = DataLayer.Cariler.Where(x => x.ID == ID).First() };
            }
        }
        private void BtnKaydet_Clicked(object sender, EventArgs e)
        {
            if (!DataLayer.IsOnline)
            {
                appSettings.UyariGoster("Cari kartı düzenlemek için online olmalısınız.");
                return;
            }
            if (!DataLayer.IsOfflineAlert)
            {
                using (GoldenContext c = new GoldenContext())
                {
                    if (viewModel.item.ID <= 0)
                    {
                        c.CRD_Cari.Add(viewModel.item);
                    }
                    else
                    {
                        c.CRD_Cari.Update(viewModel.item);
                    }

                    if (!c.SaveContextWithException()) return;
                    ID = viewModel.item.ID;
                    DataLayer.Cariler.Add(viewModel.item);
                }
                Navigation.PopAsync();
            }
        }

        async void BtnIptal_Clicked(object sender, EventArgs e)
        {
            Navigation.PopAsync();
        }

    }



}