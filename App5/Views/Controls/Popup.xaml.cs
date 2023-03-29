using GoldenMobileX.ViewModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoldenMobileX.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Popup : ContentPage
    {
        public BaseViewModel viewModel
        {
            get { return (BaseViewModel)BindingContext; }
            set { BindingContext = value; }
        }
        public Popup()
        {
            InitializeComponent();
            BindingContext = new BaseViewModel();
            this.BackgroundColor = new Color(0, 0, 0, 0.5);

        }

        public static readonly BindableProperty PopupTextProperty =
BindableProperty.Create(nameof(PopupText), typeof(string), typeof(Popup), "Yükleniyor...");

        public string PopupText
        {
            get { return (string)GetValue(PopupTextProperty); }
            set { SetValue(PopupTextProperty, value); lblLoadingText.Text = value; }
        }



        public void Run()
        {
            popupLoadingView.IsVisible = true;
            activityIndicator.IsRunning = true;
        }

        public void Stop()
        {
            popupLoadingView.IsVisible = false;
            activityIndicator.IsRunning = false;
        }
    }
}