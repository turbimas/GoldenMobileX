using GoldenMobileX.Models;
using GoldenMobileX.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoldenMobileX.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CariHesapHareketleri : ContentPage
    {
        public int CariID = 0;
        FinansViewModel viewModel
        {
            get { return (FinansViewModel)BindingContext; }
        }
        public CariHesapHareketleri()
        {
            InitializeComponent();
            BindingContext = new FinansViewModel();

        }
 
        protected override void OnAppearing()
        {
            IsBusy = true;
            viewModel.hareketler = new ObservableCollection<V_CariHareketler>(DataLayer.V_CariHareketler(CariID));

            BindingContext = new FinansViewModel() { hareketler = new ObservableCollection<V_CariHareketler>(viewModel.hareketler) };

            IsBusy = false;
        }
 
 
        private void HareketDetay_Clicked(object sender, EventArgs e)
        {

        }
    }
}