using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace GoldenMobileX.Views.Controls
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class Popup : ContentView
    {
        public Popup()
        {
            InitializeComponent();

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