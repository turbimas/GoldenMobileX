using System.Data;
using System.Linq;
using Xamarin.Forms.Xaml;

namespace GoldenMobileX.Views
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class SatinAlmaSiparisleri : Siparisler
    {
        public SatinAlmaSiparisleri()
        {
            OrderType = viewModel.types.Where(s => s.Code == 3).FirstOrDefault();

        }


    }
}