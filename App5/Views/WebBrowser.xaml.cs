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
    public partial class WebBrowser : ContentPage
    {
      
        public WebBrowser(string source)
        {
            InitializeComponent();
            WebView1.Source = source;
        }
    }
}