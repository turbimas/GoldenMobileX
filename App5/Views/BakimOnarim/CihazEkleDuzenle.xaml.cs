using GoldenMobileX.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoldenMobileX.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CihazEkleDuzenle : ContentPage
    {
        public BakimOnarimViewModel viewModel
        {
            get; set;
        }
        public CihazEkleDuzenle()
        {
            InitializeComponent();
        }
    }
}