using GoldenMobileX.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoldenMobileX.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class DurumDegistir : ContentPage
    {
        public int? Code;
        public List<X_Types> status;
        public DurumDegistir()
        {
            InitializeComponent();
        }
        private void DurumStack_Tapped(object sender, EventArgs e)
        {


            var mi = sender as StackLayout;
            X_Types t = (X_Types)((TapGestureRecognizer)mi.GestureRecognizers.First()).CommandParameter;
            Code = t.Code;

            Navigation.PopAsync();
            Appearing += DurumDegistir_Appearing;
        }

        private void DurumDegistir_Appearing(object sender, EventArgs e)
        {
            ListViewStatus.ItemsSource = status;
        }
    }
}