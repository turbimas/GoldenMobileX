using GoldenMobileX.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoldenMobileX.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Cihazlar : ContentPage
    {
        public BakimOnarimViewModel viewModel
        {
            get; set;
        }
        public Cihazlar()
        {
            InitializeComponent();
        }
    }
}